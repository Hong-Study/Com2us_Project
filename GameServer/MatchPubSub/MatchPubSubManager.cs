using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using StackExchange.Redis;

namespace GameServer;

public class MatchPubSubManager
{
    Func<bool> IsEmptyRoomFunc = null!;
    Func<Int32, bool> SetRoomStateEmptyFunc = null!;
    Func<Int32, bool> SetRoomStateWatingFunc = null!;
    

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    RedisConnector _redisConnector = null!;
    ISubscriber _subscriber = null!;
    RedisChannel _pubChannel;
    RedisChannel _subChannel;

    public MatchPubSubManager(ref readonly ServerOption option)
    {
        _redisConnector = new RedisConnector(option.RedisConnectionString);
        _subscriber = _redisConnector.RedisCon.GetConnection().GetSubscriber();

        _pubChannel = new RedisChannel(option.RedisPubKey, RedisChannel.PatternMode.Literal);
        _subChannel = new RedisChannel(option.RedisSubKey, RedisChannel.PatternMode.Literal);
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void Start()
    {
        Subscribe();
    }

    void Subscribe()
    {
        _subscriber.Subscribe(_subChannel, OnSubscribe);
    }

    void Unsubscribe()
    {
        _subscriber.Unsubscribe(_subChannel);
    }

    void Publish(string message)
    {
        _subscriber.Publish(_pubChannel, message);
    }

    void OnSubscribe(RedisChannel channel, RedisValue message)
    {
        if (RoomManager.IsExistEmptyRoom == false)
        {
            return;
        }

        if (message.HasValue == false)
        {
            return;
        }

        var matchingData = JsonSerializer.Deserialize<MatchingData>(message!);
        if (matchingData == null)
        {
            return;
        }

        if (matchingData.Type == PublishType.Matching)
        {
            // 빈 방 정보 가져오기
        }
        else if (matchingData.Type == PublishType.Complete)
        {

        }
        else
        {
            Logger.Error("Invalid PublishType");
        }
    }
}