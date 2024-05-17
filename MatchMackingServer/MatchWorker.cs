using CloudStructures;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

using Common;
using CloudStructures.Structures;
using System.Threading.Tasks.Dataflow;

public interface IMatchWoker : IDisposable
{
    public ErrorCode AddUser(string userID);
    public ErrorCode RemoveUser(string userID);
    public (bool, MatchingServerInfo?) GetCompleteMatching(string userID);
}

public class MatchWoker : IMatchWoker
{
    ILogger<MatchWoker> _logger = null!;

    System.Threading.Thread? _reqWorker = null;

    ConcurrentList<string> _reqList = new();

    Int64 _matchID = 0;

    System.Threading.Thread? _completeWorker = null;
    ConcurrentDictionary<string, MatchingServerInfo> _completeDic = new();

    RedisConnection _redisUserStateConnection = null!;

    RedisConnection _redisCompleteConnection = null!;
    RedisList<string> _redisCompleteList;

    RedisConnection _redisMatchConnection = null!;
    RedisList<string> _redisMatchList;

    string _redisAddress = "";

    string _userStateKey = "_state";

    public MatchWoker(IOptions<MatchingConfig> matchingConfig, ILogger<MatchWoker> logger)
    {
        _logger = logger;

        _redisAddress = matchingConfig.Value.RedisAddress;

        TimeSpan timeout = new(0, 0, 1);

        var config = new RedisConfig("default", _redisAddress);
        _redisUserStateConnection = new RedisConnection(config);

        _redisCompleteConnection = new RedisConnection(config);
        RedisKey redisCompleteKey = new(matchingConfig.Value.SubKey);
        _redisCompleteList = new(_redisCompleteConnection, redisCompleteKey, timeout);

        _redisMatchConnection = new RedisConnection(config);
        RedisKey redisMatchKey = new(matchingConfig.Value.PubKey);
        _redisMatchList = new(_redisMatchConnection, redisMatchKey, timeout);

        _reqWorker = new System.Threading.Thread(this.RunMatchingList);
        _reqWorker.Start();

        _completeWorker = new System.Threading.Thread(this.CompleteMatchingList);
        _completeWorker.Start();
    }

    public ErrorCode AddUser(string userID)
    {
        string key = userID + _userStateKey;

        RedisString<string> str = new(_redisUserStateConnection, key, TimeSpan.MaxValue);
        var result = str.GetAsync().Result;

        if (result.HasValue == false)
        {
            return ErrorCode.MATCHING_SERVER_ERROR;
        }

        if (result.Value != UserState.LOBBY)
        {
            return ErrorCode.MATCHING_ALEARY_MATCHED;
        }

        _ = str.SetAsync(UserState.JOIN_MATCh).Result;

        _reqList.Add(userID);

        return ErrorCode.NONE;
    }

    public ErrorCode RemoveUser(string userID)
    {
        string key = userID + _userStateKey;

        RedisString<string> str = new(_redisUserStateConnection, key, TimeSpan.MaxValue);
        var getResult = str.GetAsync().Result;

        if (getResult.HasValue == false)
        {
            return ErrorCode.MATCHING_SERVER_ERROR;
        }

        if (getResult.Value == UserState.JOIN_MATCh)
        {
            bool isSuccess = _reqList.Remove(userID);
            if (isSuccess == false)
            {
                return ErrorCode.MATCHING_ALREADY_COMPLETE;
            }

            var setResult = str.SetAsync(UserState.LOBBY).Result;
            return ErrorCode.NONE;
        }
        else if (getResult.Value == UserState.COMPLETE_MATCH)
        {
            return ErrorCode.MATCHING_ALREADY_COMPLETE;
        }

        return ErrorCode.MATCHING_NOT_FOUND_USER;
    }

    public (bool, MatchingServerInfo?) GetCompleteMatching(string userID)
    {
        // 유저가 오랫동안 체크 안하는 경우도 있을 수 있다.
        if (_completeDic.TryGetValue(userID, out MatchingServerInfo? data))
        {
            _completeDic.Remove(userID, out _);
            return (true, data);
        }

        return (false, null);
    }

    void RunMatchingList()
    {
        while (true)
        {
            try
            {
                if (_reqList.Count < 2)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                if (_reqList.TryGetFirstAndDelete(out var user1) == false)
                {
                    continue;
                }

                if (_reqList.TryGetFirstAndDelete(out var user2) == false)
                {
                    _reqList.Add(user1);
                    continue;
                }

                // 강제 종료한 유저일 수도 있다. 이건 어떻게 판별할 것인가? 말까?
                {
                    string key = user1 + _userStateKey;

                    RedisString<string> redisString = new(_redisUserStateConnection, key, TimeSpan.FromSeconds(10));
                    var value = redisString.SetAsync(UserState.COMPLETE_MATCH).Result;
                }

                {
                    string key = user2 + _userStateKey;

                    RedisString<string> redisString = new(_redisUserStateConnection, key, TimeSpan.FromSeconds(10));
                    var value = redisString.SetAsync(UserState.COMPLETE_MATCH).Result;
                }

                Int64 id = Interlocked.Increment(ref _matchID);

                MatchingData matchingData = new()
                {
                    MatchID = id,
                    FirstUserID = user1,
                    SecondUserID = user2
                };

                var result = _redisMatchList.LeftPushAsync(JsonSerializer.Serialize(matchingData)).Result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RunMatchingList Error");
            }
        }
    }

    void CompleteMatchingList()
    {
        while (true)
        {
            try
            {
                var result = _redisCompleteList.RightPopAsync().Result;
                if (result.HasValue == false)
                {
                    continue;
                }

                var data = JsonSerializer.Deserialize<CompleteMatchingData>(result.Value);
                if (data == null)
                    return;

                if (_completeDic.ContainsKey(data.FirstUserID) || _completeDic.ContainsKey(data.SecondUserID))
                {
                    return;
                }

                _completeDic.TryAdd(data.FirstUserID, data.ServerInfo);
                _completeDic.TryAdd(data.SecondUserID, data.ServerInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CompleteMatchingList Error");
            }
        }
    }

    public void Dispose()
    {

    }
}

public class MatchingConfig
{
    public string RedisAddress { get; set; } = null!;
    public string PubKey { get; set; } = null!;
    public string SubKey { get; set; } = null!;
}