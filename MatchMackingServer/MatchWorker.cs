using CloudStructures;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;

using Common;

public interface IMatchWoker : IDisposable
{
    public ErrorCode AddUser(string userID);

    public (bool, MatchingServerInfo?) GetCompleteMatching(string userID);
}

public class MatchWoker : IMatchWoker
{
    ILogger<MatchWoker> _logger = null!;

    System.Threading.Thread? _reqWorker = null;
    ConcurrentQueue<string> _reqQueue = new();

    System.Threading.Thread? _completeWorker = null;

    // key는 유저ID
    ConcurrentDictionary<string, MatchingServerInfo> _completeDic = new();

    //TODO: 2개의 Pub/Sub을 사용하므로 Redis 객체가 2개 있어야 한다.
    // 매칭서버에서 -> 게임서버, 게임서버 -> 매칭서버로
    RedisConnection _redisSubConnection = null!;
    ISubscriber _redisSubscriber = null!;

    RedisConnection _redisPubConnection = null!;
    ISubscriber _redisPubscriber = null!;

    string _redisAddress = "";
    RedisChannel _matchingRedisPubChannel;
    RedisChannel _matchingRedisSubChannel;

    public MatchWoker(IOptions<MatchingConfig> matchingConfig, ILogger<MatchWoker> logger)
    {
        _logger = logger;

        _redisAddress = matchingConfig.Value.RedisAddress;
        _matchingRedisPubChannel = new(matchingConfig.Value.PubKey, RedisChannel.PatternMode.Literal);
        _matchingRedisSubChannel = new(matchingConfig.Value.SubKey, RedisChannel.PatternMode.Literal);

        //TODO: Redis 연결 및 초기화 한다
        _redisSubConnection = new RedisConnection(new("default", _redisAddress));
        _redisSubscriber = _redisSubConnection.GetConnection().GetSubscriber();

        _redisPubConnection = new RedisConnection(new("default", _redisAddress));
        _redisPubscriber = _redisPubConnection.GetConnection().GetSubscriber();

        _reqWorker = new System.Threading.Thread(this.RunMatching);
        _reqWorker.Start();

        _redisSubscriber.Subscribe(_matchingRedisSubChannel, RunMatchingComplete);

        System.Console.WriteLine("MatchWoker 생성자 호출");
    }


    public ErrorCode AddUser(string userID)
    {
        _reqQueue.Enqueue(userID);

        return ErrorCode.NONE;
    }

    public (bool, MatchingServerInfo?) GetCompleteMatching(string userID)
    {
        //TODO: _completeDic에서 검색해서 있으면 반환한다.
        if (_completeDic.TryGetValue(userID, out MatchingServerInfo? data))
        {
            return (true, data);
        }

        return (false, null);
    }

    void RunMatching()
    {
        while (true)
        {
            try
            {
                if (_reqQueue.Count < 2)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                //TODO: 큐에서 2명을 가져온다. 두명을 매칭시킨다
                if (!_reqQueue.TryDequeue(out string? user1))
                {
                    continue;
                }

                if (!_reqQueue.TryDequeue(out string? user2))
                {
                    _reqQueue.Enqueue(user1);
                    continue;
                }

                //TODO: Redis의 Pub/Sub을 이용해서 매칭된 유저들을 게임서버에 전달한다.
                MatchingUserData matchingUserData = new()
                {
                    FirstUserID = user1,
                    SecondUserID = user2,
                };

                MatchingData matchingData = new()
                {
                    Type = PublishType.Matching,
                    MatchingUserData = matchingUserData,
                };

                _redisPubscriber.Publish(_matchingRedisPubChannel, JsonSerializer.Serialize(matchingData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RunMatching Error");
            }
        }
    }

    void RunMatchingComplete(RedisChannel channel, RedisValue value)
    {
        try
        {
            if (value.HasValue == false)
                return;

            var data = JsonSerializer.Deserialize<CompleteMatchingData>(value!);
            if (data == null)
                return;

            //TODO: 매칭 결과를 _completeDic에 넣는다
            // 2명이 하므로 각각 유저를 대상으로 총 2개를 _completeDic에 넣어야 한다
            if (_completeDic.TryAdd(data.FirstUserID, data.ServerInfo) == false)
            {
                return;
            }

            _completeDic.TryAdd(data.SecondUserID, data.ServerInfo);

            MatchingData publishData = new()
            {
                Type = PublishType.Complete,
                MatchingServerInfo = data.ServerInfo,
            };

            _redisPubscriber.Publish(_matchingRedisPubChannel, JsonSerializer.Serialize(publishData));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunMatchingComplete Error");
        }
    }

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