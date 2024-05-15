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

    // key는 유저ID
    ConcurrentDictionary<string, MatchingServerInfo> _completeDic = new();
    Int64 _matchID = 0;

    //TODO: 2개의 Pub/Sub을 사용하므로 Redis 객체가 2개 있어야 한다.
    // 매칭서버에서 -> 게임서버, 게임서버 -> 매칭서버로
    RedisConnection _redisSubConnection = null!;
    RedisList<string> _redisCompleteList;
    ISubscriber _redisSubscriber = null!;

    RedisConnection _redisPubConnection = null!;
    RedisList<string> _redisMatchList;
    // ISubscriber _redisPubscriber = null!;

    string _redisAddress = "";
    // RedisChannel _matchingRedisPubChannel;
    // RedisChannel _matchingRedisSubChannel;

    object _lock = new object();

    public MatchWoker(IOptions<MatchingConfig> matchingConfig, ILogger<MatchWoker> logger)
    {
        _logger = logger;

        _redisAddress = matchingConfig.Value.RedisAddress;

        // RedisList 초기화
        TimeSpan timeout = new(0, 0, 1);

        _redisSubConnection = new RedisConnection(new("default", _redisAddress));
        RedisKey redisCompleteKey = new(matchingConfig.Value.SubKey);
        _redisCompleteList = new(_redisSubConnection, redisCompleteKey, timeout);

        _redisPubConnection = new RedisConnection(new("default", _redisAddress));
        RedisKey redisMatchKey = new(matchingConfig.Value.PubKey);
        _redisMatchList = new(_redisPubConnection, redisMatchKey, timeout);

        // _matchingRedisPubChannel = new(matchingConfig.Value.PubKey, RedisChannel.PatternMode.Literal);
        // _matchingRedisSubChannel = new(matchingConfig.Value.SubKey, RedisChannel.PatternMode.Literal);

        //TODO: Redis 연결 및 초기화 한다
        // _redisSubConnection = new RedisConnection(new("default", _redisAddress));
        // _redisSubscriber = _redisSubConnection.GetConnection().GetSubscriber();

        // _redisPubConnection = new RedisConnection(new("default", _redisAddress));
        // _redisPubscriber = _redisPubConnection.GetConnection().GetSubscriber();

        _reqWorker = new System.Threading.Thread(this.RunMatchingList);
        _reqWorker.Start();

        _completeWorker = new System.Threading.Thread(this.CompleteMatchingList);
        _completeWorker.Start();

        // _redisSubscriber.Subscribe(_matchingRedisSubChannel, RunMatchingComplete);

        System.Console.WriteLine("MatchWoker 생성자 호출");
    }

    public ErrorCode AddUser(string userID)
    {
        lock (_lock)
        {
            if (_reqList.Find(x => x == userID) != null)
            {   
                return ErrorCode.MATCHING_ALEARY_MATCHED;
            }
            _logger.LogInformation($"AddUser: {userID}");
            
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
        //TODO: _completeDic에서 검색해서 있으면 반환한다.
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

                //TODO: 큐에서 2명을 가져온다. 두명을 매칭시킨다
                Int64 id = Interlocked.Increment(ref _matchID);

                MatchingUserData matchingUserData = new()
                {
                    FirstUserID = user1,
                    SecondUserID = user2,
                };

                MatchingData matchingData = new()
                {
                    Type = PublishType.Matching,
                    MatchID = id,
                    MatchingUserData = matchingUserData,
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

                //TODO: 매칭 결과를 _completeDic에 넣는다
                // 2명이 하므로 각각 유저를 대상으로 총 2개를 _completeDic에 넣어야 한다
                if (_completeDic.ContainsKey(data.FirstUserID) || _completeDic.ContainsKey(data.SecondUserID))
                {
                    return;
                }

                System.Console.WriteLine($"CompleteMatchingList 호출: {data.FirstUserID}, {data.SecondUserID}");

                _completeDic.TryAdd(data.FirstUserID, data.ServerInfo);
                _completeDic.TryAdd(data.SecondUserID, data.ServerInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CompleteMatchingList Error");
            }
        }
    }


    // void RunMatchingPubSub()
    // {
    //     while (true)
    //     {
    //         try
    //         {
    //             if (_reqQueue.Count < 2)
    //             {
    //                 System.Threading.Thread.Sleep(1);
    //                 continue;
    //             }

    //             //TODO: 큐에서 2명을 가져온다. 두명을 매칭시킨다
    //             if (!_reqQueue.TryDequeue(out string? user1))
    //             {
    //                 continue;
    //             }

    //             if (!_reqQueue.TryDequeue(out string? user2))
    //             {
    //                 _reqQueue.Enqueue(user1);
    //                 continue;
    //             }

    //             //TODO: Redis의 Pub/Sub을 이용해서 매칭된 유저들을 게임서버에 전달한다.
    //             Int64 id = Interlocked.Increment(ref _matchID);

    //             MatchingUserData matchingUserData = new()
    //             {
    //                 FirstUserID = user1,
    //                 SecondUserID = user2,
    //             };

    //             MatchingData matchingData = new()
    //             {
    //                 Type = PublishType.Matching,
    //                 MatchID = id,
    //                 MatchingUserData = matchingUserData,
    //             };

    //             _redisPubscriber.Publish(_matchingRedisPubChannel, JsonSerializer.Serialize(matchingData));
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError(ex, "RunMatching Error");
    //         }
    //     }
    // }

    // void RunMatchingCompletePubSub(RedisChannel channel, RedisValue value)
    // {
    //     try
    //     {
    //         if (value.HasValue == false)
    //             return;

    //         var data = JsonSerializer.Deserialize<CompleteMatchingData>(value!);
    //         if (data == null)
    //             return;

    //         //TODO: 매칭 결과를 _completeDic에 넣는다
    //         // 2명이 하므로 각각 유저를 대상으로 총 2개를 _completeDic에 넣어야 한다
    //         if (_completeDic.ContainsKey(data.FirstUserID) || _completeDic.ContainsKey(data.SecondUserID))
    //         {
    //             return;
    //         }

    //         MatchingData publishData = new()
    //         {
    //             Type = PublishType.Complete,
    //             MatchID = data.MatchID,
    //             MatchingServerInfo = data.ServerInfo,
    //         };

    //         _redisPubscriber.Publish(_matchingRedisPubChannel, JsonSerializer.Serialize(publishData));

    //         _completeDic.TryAdd(data.FirstUserID, data.ServerInfo);
    //         _completeDic.TryAdd(data.SecondUserID, data.ServerInfo);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "RunMatchingComplete Error");
    //     }
    // }

    public void Dispose()
    {
        _redisSubscriber.UnsubscribeAll();

        Console.WriteLine("MatchWoker 소멸자 호출");
    }
}

public class MatchingConfig
{
    public string RedisAddress { get; set; } = null!;
    public string PubKey { get; set; } = null!;
    public string SubKey { get; set; } = null!;
}