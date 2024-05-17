using System.Text.Json;
using CloudStructures.Structures;

namespace GameServer;

public class MatchWorker
{
    RedisConnector _redisMatchConnection = null!;
    RedisConnector _redisCompleteConnection = null!;

    RedisList<string> _matchList;
    RedisList<string> _completeList;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    Action<ServerPacketData> _sendInnerFunc = null!;

    string _serverAddress = "";
    Int32 _serverPort;

    List<UsingRoomInfo> _usingRoomInfos = new List<UsingRoomInfo>();

    public MatchWorker(string serverAddress, Int32 serverPort, string connectionString, string matchKey, string completeKey)
    {
        _serverAddress = serverAddress;
        _serverPort = serverPort;

        _redisMatchConnection = new RedisConnector(connectionString);
        _redisCompleteConnection = new RedisConnector(connectionString);

        TimeSpan timeout = TimeSpan.MaxValue;

        _matchList = new RedisList<string>(_redisMatchConnection.RedisCon, matchKey, timeout);
        _completeList = new RedisList<string>(_redisCompleteConnection.RedisCon, completeKey, timeout);
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void SetInnerFunc(Action<ServerPacketData> sendInnerFunc)
    {
        _sendInnerFunc = sendInnerFunc;
    }

    public void InitUsingRoomList(ref readonly RoomManager roomManager)
    {
        roomManager.InitUsingRoomList(ref _usingRoomInfos);
    }

    public void SetEmptyRoom(Int32 roomNumber)
    {
        var emptyRoomInfo = _usingRoomInfos.Find(x => x.RoomNumber == roomNumber);
        if (emptyRoomInfo == null)
        {
            Logger.Error("EmptyRoomInfo is null");
            return;
        }

        emptyRoomInfo.RoomState = RoomState.Empty;
    }

    public void Process()
    {
        if (!IsEmptyRoom())
        {
            Thread.Sleep(1);
            return;
        }

        var matchMessage = _matchList.RightPopAsync().Result;
        if (matchMessage.HasValue == true)
        {
            OnMatchingHandle(matchMessage.Value);
        }
        else
        {
            Thread.Sleep(1);
        }
    }

    void OnMatchingHandle(string result)
    {        
        var matchingData = JsonSerializer.Deserialize<MatchingData>(result);
        if (matchingData == null)
        {
            Logger.Error("MatchingData is null");
            return;
        }

        var emptyRoomInfo = GetEmptyRoom();

        Logger.Info($"Matching Success RoomID : {emptyRoomInfo.RoomNumber}");

        emptyRoomInfo.RoomState = RoomState.Waiting;

        NTFMatchingReq req = new NTFMatchingReq();
        req.RoomNumber = emptyRoomInfo.RoomNumber;
        req.FirstUserID = matchingData.FirstUserID;
        req.SecondUserID = matchingData.SecondUserID;

        var data = PacketManager.MakeInnerPacket("", req, InnerPacketType.NTF_REQ_MATCHING_ROOM);
        _sendInnerFunc(data);

        var completeMatchingData = new CompleteMatchingData();
        completeMatchingData.MatchID = matchingData.MatchID;
        completeMatchingData.FirstUserID = matchingData.FirstUserID;
        completeMatchingData.SecondUserID = matchingData.SecondUserID;
        completeMatchingData.ServerInfo = new MatchingServerInfo()
        {
            ServerAddress = _serverAddress,
            Port = _serverPort,
            RoomNumber = emptyRoomInfo.RoomNumber
        };

        var pushResult = _completeList.LeftPushAsync(JsonSerializer.Serialize(completeMatchingData)).Result;
        if (pushResult == 0)
        {
            Logger.Error("CompleteList LeftPush Fail");
        }
    }

    bool IsEmptyRoom()
    {
        return _usingRoomInfos.Exists(x => x.RoomState == RoomState.Empty);
    }

    UsingRoomInfo GetEmptyRoom()
    {
        return _usingRoomInfos.Find(x => x.RoomState == RoomState.Empty)!;
    }
}