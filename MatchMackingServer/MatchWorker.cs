using CloudStructures;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;

using Common;
using CloudStructures.Structures;

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
    List<string> _reqList = new();

    System.Threading.Thread? _completeWorker = null;

    ConcurrentDictionary<string, MatchingServerInfo> _completeDic = new();
    Int64 _matchID = 0;

    RedisConnection _redisSubConnection = null!;
    RedisList<string> _redisCompleteList;
    ISubscriber _redisSubscriber = null!;

    RedisConnection _redisPubConnection = null!;
    RedisList<string> _redisMatchList;

    string _redisAddress = "";

    object _lock = new object();

    public MatchWoker(IOptions<MatchingConfig> matchingConfig, ILogger<MatchWoker> logger)
    {
        _logger = logger;

        _redisAddress = matchingConfig.Value.RedisAddress;

        TimeSpan timeout = new(0, 0, 1);

        _redisSubConnection = new RedisConnection(new("default", _redisAddress));
        RedisKey redisCompleteKey = new(matchingConfig.Value.SubKey);
        _redisCompleteList = new(_redisSubConnection, redisCompleteKey, timeout);

        _redisPubConnection = new RedisConnection(new("default", _redisAddress));
        RedisKey redisMatchKey = new(matchingConfig.Value.PubKey);
        _redisMatchList = new(_redisPubConnection, redisMatchKey, timeout);

        _reqWorker = new System.Threading.Thread(this.RunMatchingList);
        _reqWorker.Start();

        _completeWorker = new System.Threading.Thread(this.CompleteMatchingList);
        _completeWorker.Start();
    }

    public ErrorCode AddUser(string userID)
    {
        lock (_lock)
        {
            if (_reqList.Find(x => x == userID) != null)
            {
                return ErrorCode.MATCHING_ALEARY_MATCHED;
            }

            _reqList.Add(userID);
        }

        return ErrorCode.NONE;
    }

    public ErrorCode RemoveUser(string userID)
    {
        lock (_lock)
        {
            if (_reqList.Find(x => x == userID) != null)
            {
                _reqList.Remove(userID);
                return ErrorCode.NONE;
            }
        }

        return ErrorCode.MATCHING_NOT_FOUND_USER;
    }

    public (bool, MatchingServerInfo?) GetCompleteMatching(string userID)
    {
        if (_completeDic.TryGetValue(userID, out MatchingServerInfo? data))
        {
            _completeDic.Remove(userID, out _);
            return (true, data);
        }

        return (false, null);
    }

    async void RunMatchingList()
    {
        while (true)
        {
            try
            {
                string user1 = "";
                string user2 = "";
                lock (_lock)
                {
                    if (_reqList.Count() < 2)
                    {
                        System.Threading.Thread.Sleep(1);
                        continue;
                    }

                    user1 = _reqList.First();
                    _reqList.RemoveAt(0);
                    user2 = _reqList.First();
                    _reqList.RemoveAt(0);
                }

                Int64 id = Interlocked.Increment(ref _matchID);

                MatchingData matchingData = new()
                {
                    Type = PublishType.Matching,
                    MatchID = id,
                    FirstUserID = user1,
                    SecondUserID = user2
                };

                await _redisMatchList.LeftPushAsync(JsonSerializer.Serialize(matchingData));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RunMatchingList Error");
            }
        }
    }

    async void CompleteMatchingList()
    {
        while (true)
        {
            try
            {
                var result = await _redisCompleteList.RightPopAsync();
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