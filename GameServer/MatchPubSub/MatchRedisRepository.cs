using System.Text.Json;
using CloudStructures.Structures;

namespace GameServer;

public class MatchRedisRepository
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

    public MatchRedisRepository(string serverAddress, Int32 serverPort, string connectionString, string matchKey, string completeKey)
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

    public void SetEmptyRoom(Int32 roomID)
    {
        var emptyRoomInfo = _usingRoomInfos.Find(x => x.RoomID == roomID);
        if (emptyRoomInfo == null)
        {
            Logger.Error("EmptyRoomInfo is null");
            return;
        }

        emptyRoomInfo.RoomState = RoomState.Empty;
    }

    public async Task Process()
    {
        var matchMessage = await _matchList.RightPopAsync();
        if (matchMessage.HasValue == true)
        {
            await OnMatchingHandle(matchMessage.Value);
        }
    }

    async Task OnMatchingHandle(string result)
    {
        System.Console.WriteLine("OnMatchingHandle : " + result);
        
        var matchingData = JsonSerializer.Deserialize<MatchingData>(result);
        if (matchingData == null)
        {
            Logger.Error("MatchingData is null");
            return;
        }

        if (matchingData.MatchingUserData == null)
        {
            Logger.Error("MatchingUserData is null");
            return;
        }

        var emptyRoomInfo = GetEmptyRoom();
        if (emptyRoomInfo == null)
        {
            // TODO
            Logger.Error("EmptyRoomInfo is null");
            return;
        }

        Logger.Info($"Matching Success RoomID : {emptyRoomInfo.RoomID}");

        emptyRoomInfo.RoomState = RoomState.Waiting;

        NTFMatchingReq req = new NTFMatchingReq();
        req.RoomID = emptyRoomInfo.RoomID;
        req.FirstUserID = matchingData.MatchingUserData.FirstUserID;
        req.SecondUserID = matchingData.MatchingUserData.SecondUserID;

        var data = PacketManager.MakeInnerPacket("", req, InnerPacketType.NTF_REQ_MATCHING_ROOM);
        _sendInnerFunc(data);

        var completeMatchingData = new CompleteMatchingData();
        completeMatchingData.MatchID = matchingData.MatchID;
        completeMatchingData.FirstUserID = matchingData.MatchingUserData.FirstUserID;
        completeMatchingData.SecondUserID = matchingData.MatchingUserData.SecondUserID;
        completeMatchingData.ServerInfo = new MatchingServerInfo()
        {
            ServerAddress = _serverAddress,
            Port = _serverPort,
            RoomNumber = emptyRoomInfo.RoomID
        };

        await _completeList.LeftPushAsync(JsonSerializer.Serialize(completeMatchingData));
    }

    UsingRoomInfo? GetEmptyRoom()
    {
        return _usingRoomInfos.Find(x => x.RoomState == RoomState.Empty);
    }
}