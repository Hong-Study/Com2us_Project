using MemoryPack;
using Common;
using System.Threading.Tasks.Dataflow;

namespace GameServer;

public class PacketManager
{
    public PacketHandler _handler = new PacketHandler();
    Dictionary<Int16, Action<ServerPacketData>> _onRecv = new Dictionary<Int16, Action<ServerPacketData>>();
    Dictionary<Int16, Action<string, IMessage>> _onHandler = new Dictionary<Int16, Action<string, IMessage>>();
    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public PacketManager()
    {
        InitHandler();
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _handler.InitLogger(logger);
    }

    public void InitHandler()
    {
        _onRecv.Add((Int16)PacketType.RES_C_PONG, Make<CPongRes>);
        _onHandler.Add((Int16)PacketType.RES_C_PONG, _handler.Handle_C_Pong);

        _onRecv.Add((Int16)PacketType.REQ_C_LOGIN, Make<CLoginReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_LOGIN, _handler.Handle_C_Login);
        _onRecv.Add((Int16)PacketType.REQ_C_LOGOUT, Make<CLogOutReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_LOGOUT, _handler.Handle_C_Logout);

        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_ENTER, Make<CRoomEnterReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_ENTER, _handler.Handle_C_RoomEnter);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, Make<CRoomLeaveReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, _handler.Handle_C_RoomLeave);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_CHAT, Make<CRoomChatReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_CHAT, _handler.Handle_C_RoomChat);

        _onRecv.Add((Int16)PacketType.REQ_C_GAME_READY, Make<CGameReadyReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_READY, _handler.Handle_C_GameReady);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_START, Make<CGameStartRes>);
        _onHandler.Add((Int16)PacketType.RES_S_GAME_READY, _handler.Handle_C_GameStart);
        _onRecv.Add((Int16)PacketType.REQ_C_GAME_PUT, Make<CGamePutReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_PUT, _handler.Handle_C_GamePut);
        _onRecv.Add((Int16)PacketType.RES_C_TURN_CHANGE, Make<CTurnChangeRes>);
        _onHandler.Add((Int16)PacketType.RES_C_TURN_CHANGE, _handler.Handle_C_TurnChange);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_END, Make<CGameEndRes>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_END, _handler.Handle_C_GameEnd);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_CANCLE, Make<CGameCancleRes>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_CANCLE, _handler.Handle_C_GameCancle);

        _onRecv.Add((Int16)InnerPacketType.NTF_SESSION_CONNECTED, Make<NTFSessionConnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_CONNECTED, _handler.Handle_NTF_SessionConnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, Make<NTFSessionDisconnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, _handler.Handle_NTF_SessionDisconnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, Make<NTFCheckSessionLoginReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, _handler.Handle_NTF_CheckSessionLogin);
        _onRecv.Add((Int16)InnerPacketType.NTF_HEART_BEAT, Make<NTFHeartBeatReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_HEART_BEAT, _handler.Handle_NTF_HeartBeat);
        _onRecv.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, Make<NTFRoomsCheckReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, _handler.Handle_NTF_RoomsCheck);
        _onRecv.Add((Int16)InnerPacketType.NTF_RES_USER_LOGIN, Make<NTFUserLoginRes>);
        _onHandler.Add((Int16)InnerPacketType.NTF_RES_USER_LOGIN, _handler.Handle_NTF_UserLogin);
        _onRecv.Add((Int16)InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT, Make<NTFUserWinLoseUpdateRes>);
        _onHandler.Add((Int16)InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT, _handler.Handle_NTF_UpdateWinLoseCount);
    }

    public void SetUserDelegate(ref readonly UserManager userManager)
    {
        _handler.AddUserFunc = userManager.AddUser;
        _handler.RemoveUserFunc = userManager.RemoveUser;
        _handler.LoginUserFunc = userManager.LoginUser;
        _handler.GetUserFunc = userManager.GetUserInfo;
        _handler.SessionLoginTimeoutCheckFunc = userManager.SessionLoginTimeoutCheck;
        _handler.HeartHeatCheckFunc = userManager.HeartBeatCheck;
        _handler.ReceivePongFunc = userManager.ReceivePong;
    }

    public void SetRoomDelegate(ref readonly RoomManager roomManager)
    {
        _handler.GetRoomFunc = roomManager.GetRoom;
        _handler.RoomCheckFunc = roomManager.RoomsCheck;
        _handler.GetRoomStateFunc = roomManager.GetRoomState;
    }

    public void SetMainDelegate(ref readonly MainServer server)
    {
        _handler.SendFunc = server.SendData;
        _handler.InnerSendFunc = server.PacketInnerSend;
        _handler.DatabaseSendFunc = server.PacketDatabaseSend;
        _handler.RedisSendFunc = server.PacketRedisSend;
    }

    public void Distribute(ServerPacketData data)
    {
        _msgBuffer.Post(data);
    }

    public void Start(Int32 threadCount = 1)
    {
        for (Int32 i = 0; i < threadCount; i++)
        {
            Thread thread = new Thread(this.Process);
            thread.Start();
            _logicThreads.Add(thread);
        }
    }

    public void Stop()
    {
        foreach (var thread in _logicThreads)
        {
            thread.Join();
        }
    }

    void Process()
    {
        while (MainServer.IsRunning)
        {
            try
            {
                TimeSpan timeOut = TimeSpan.FromSeconds(1);
                ServerPacketData data = _msgBuffer.Receive(timeOut);
                
                if (_onRecv.TryGetValue(data.PacketType, out var action))
                {
                    action(data);
                }
                else
                {
                    Logger.Error($"Not found handler : {data.PacketType}");
                }
            }
            catch (TimeoutException)
            {
            }
        }
    }

    public static byte[] PacketSerialized<I, E>(I packet, E type)
        where I : IMessage
        where E : Enum
    {
        byte[]? bodyData = MemoryPackSerializer.Serialize(packet);
        Int16 bodyDataSize = 0;
        if (bodyData != null)
        {
            bodyDataSize = (Int16)bodyData.Length;
        }

        var packetSize = (Int16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

        var dataSource = new byte[packetSize];

        FastBinaryWrite.Int16(dataSource, 0, packetSize);
        FastBinaryWrite.Int16(dataSource, 2, (Int16)(object)type);
        dataSource[4] = 0;

        if (bodyData != null)
        {
            FastBinaryWrite.Bytes(dataSource, PacketDef.PACKET_HEADER_SIZE, bodyData);
        }

        return dataSource;
    }

    void Make<T>(ServerPacketData data) where T : IMessage, new()
    {
        var packet = MemoryPackSerializer.Deserialize<T>(data.Body);
        if (packet == null)
        {
            return;
        }

        if (_onHandler.TryGetValue(data.PacketType, out var action))
        {
            action(data.sessionID, packet);
        }
    }

    public static ServerPacketData MakeInnerPacket<T>(string sessionID, T packet, InnerPacketType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }

    public static ServerPacketData MakeInnerPacket<T>(string sessionID, T packet, PacketType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}