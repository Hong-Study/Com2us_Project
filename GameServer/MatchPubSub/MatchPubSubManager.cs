using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using CloudStructures.Structures;
using StackExchange.Redis;

namespace GameServer;

public class MatchPubSubManager
{
    Func<Int32, bool> SetRoomStateEmptyFunc = null!;
    // Func<Int32, bool> SetRoomStateWatingFunc = null!;
    Func<Int32, bool> SetRoomStateMathcingFunc = null!;

    Func<UsingRoomInfo?> GetEmptyRoomFunc = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    RedisConnector _redisConnector = null!;

    ISubscriber _subscriber = null!;
    RedisChannel _pubChannel;
    RedisChannel _subChannel;

    Dictionary<Int64, MatchingServerInfo> _successMatchingServerInfos = new Dictionary<Int64, MatchingServerInfo>();

    string _serverAddress = "";
    Int32 _serverPort = 0;

    List<UsingRoomInfo> _usingRoomInfos = new List<UsingRoomInfo>();

    public MatchPubSubManager(ref readonly ServerOption option)
    {
        _redisConnector = new RedisConnector(option.RedisConnectionString);

        _subscriber = _redisConnector.RedisCon.GetConnection().GetSubscriber();
        _pubChannel = new RedisChannel(option.RedisPubKey, RedisChannel.PatternMode.Literal);
        _subChannel = new RedisChannel(option.RedisSubKey, RedisChannel.PatternMode.Literal);

        _serverAddress = "127.0.0.1";
        _serverPort = option.Port;
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void SetRoomDelegate(ref readonly RoomManager roomManager)
    {
        // SetRoomStateEmptyFunc = roomManager.SetRoomStateEmpty;
        // SetRoomStateMathcingFunc = roomManager.SetRoomStateMathcing;
        // SetRoomStateWatingFunc = roomManager.SetRoomStateWaiting;

        // GetEmptyRoomFunc = roomManager.GetEmptyRoom;
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
            OnMatchList(matchingData);
        }
        else if (matchingData.Type == PublishType.Complete)
        {
            OnMatchComplete(matchingData);
        }
        else
        {
            Logger.Error("Invalid PublishType");
        }
    }

    void OnMatchList(MatchingData matchingData)
    {
        // 빈 방 정보 가져오기
        var roomInfo = GetEmptyRoomFunc();
        if (roomInfo == null)
        {
            // 빈 방이 없으니 Subscribe를 종료하거나? Sleep을 한다.
            return;
        }

        // bool isSuccess = SetRoomStateWatingFunc(roomInfo.RoomID);
        // bool isSuccess = true;

        // 매칭 성공 메시지 전송
        var completeMatchingData = new CompleteMatchingData();
        completeMatchingData.MatchID = matchingData.MatchID;
        completeMatchingData.FirstUserID = matchingData.MatchingUserData!.FirstUserID;
        completeMatchingData.SecondUserID = matchingData.MatchingUserData!.SecondUserID;
        completeMatchingData.ServerInfo = new MatchingServerInfo()
        {
            ServerAddress = _serverAddress,
            Port = _serverPort,
            RoomNumber = roomInfo.RoomID
        };

        _successMatchingServerInfos.Add(matchingData.MatchID, completeMatchingData.ServerInfo);

        Publish(JsonSerializer.Serialize(completeMatchingData));
    }

    void OnMatchComplete(MatchingData matchingData)
    {
        // 매칭 완료 메시지 전송
        // 매칭 완료 메시지 처리
        if (_successMatchingServerInfos.Count == 0)
        {
            return;
        }

        if (matchingData.MatchingServerInfo == null)
        {
            Logger.Error("MatchingServerInfo is null");
            return;
        }

        if (_successMatchingServerInfos.TryGetValue(matchingData.MatchID, out var matchingServerInfo))
        {
            // 매칭 성공한 서버 정보를 이용하여 게임 서버에 매칭 완료 메시지 전송
            // 매칭 완료 메시지를 받은 게임 서버는 매칭 완료 처리를 한다.
            if (matchingServerInfo.Port == matchingData.MatchingServerInfo.Port &&
                matchingServerInfo.RoomNumber == matchingData.MatchingServerInfo.RoomNumber)
            {
                // 매칭 완료 처리
                SetRoomStateMathcingFunc(matchingServerInfo.RoomNumber);

                _successMatchingServerInfos.Remove(matchingData.MatchID);
            }
            else
            {
                // 매칭 실패 처리
                SetRoomStateEmptyFunc(matchingData.MatchingServerInfo.RoomNumber);
            }

            _successMatchingServerInfos.Remove(matchingData.MatchID);
        }
    }
}