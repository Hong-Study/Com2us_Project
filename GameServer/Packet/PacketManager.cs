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
        _onHandler.Add((Int16)PacketType.RES_C_PONG, _handler.HandleCPong);

        _onRecv.Add((Int16)PacketType.REQ_C_LOGIN, Make<CLoginReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_LOGIN, _handler.HandleCLogin);
        _onRecv.Add((Int16)PacketType.REQ_C_LOGOUT, Make<CLogOutReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_LOGOUT, _handler.HandleCLogout);

        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_ENTER, Make<CRoomEnterReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_ENTER, _handler.HandleCRoomEnter);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, Make<CRoomLeaveReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, _handler.HandleCRoomLeave);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_CHAT, Make<CRoomChatReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_CHAT, _handler.HandleCRoomChat);

        _onRecv.Add((Int16)PacketType.REQ_C_GAME_READY, Make<CGameReadyReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_READY, _handler.HandleCGameReady);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_START, Make<CGameStartRes>);
        _onHandler.Add((Int16)PacketType.RES_S_GAME_READY, _handler.HandleCGameStart);
        _onRecv.Add((Int16)PacketType.REQ_C_GAME_PUT, Make<CGamePutReq>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_PUT, _handler.HandleCGamePut);
        _onRecv.Add((Int16)PacketType.RES_C_TURN_CHANGE, Make<CTurnChangeRes>);
        _onHandler.Add((Int16)PacketType.RES_C_TURN_CHANGE, _handler.HandleCTurnChange);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_END, Make<CGameEndRes>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_END, _handler.HandleCGameEnd);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_CANCLE, Make<CGameCancleRes>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_CANCLE, _handler.HandleCGameCancle);

        _onRecv.Add((Int16)InnerPacketType.NTF_SESSION_CONNECTED, Make<NTFSessionConnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_CONNECTED, _handler.HandleNTFSessionConnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, Make<NTFSessionDisconnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, _handler.HandleNTFSessionDisconnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, Make<NTFCheckSessionLoginReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, _handler.HandleNTFCheckSessionLogin);
        _onRecv.Add((Int16)InnerPacketType.NTF_HEART_BEAT, Make<NTFHeartBeatReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_HEART_BEAT, _handler.HandleNTFHeartBeat);
        _onRecv.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, Make<NTFRoomsCheckReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, _handler.HandleNTFRoomsCheck);
        _onRecv.Add((Int16)InnerPacketType.NTF_RES_USER_LOGIN, Make<NTFUserLoginRes>);
        _onHandler.Add((Int16)InnerPacketType.NTF_RES_USER_LOGIN, _handler.HandleNTFUserLogin);
        _onRecv.Add((Int16)InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT, Make<NTFUserWinLoseUpdateRes>);
        _onHandler.Add((Int16)InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT, _handler.HandleNTFUpdateWinLoseCount);

        _onRecv.Add((Int16)InnerPacketType.NTF_REQ_MATCHING_ROOM, Make<NTFMatchingReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_REQ_MATCHING_ROOM, _handler.HandleNTFMatchingRoom);
        _onRecv.Add((Int16)InnerPacketType.NTF_USER_DISCONNECTED, Make<NTFUserDisconnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_USER_DISCONNECTED, _handler.HandleNTFUserDisconnected);
    }

    public void SetUserDelegate(in UserManager userManager)
    {
        _handler.AddUserFunc = userManager.AddUser;
        _handler.RemoveUserFunc = userManager.RemoveUser;
        _handler.LoginUserFunc = userManager.LoginUser;
        _handler.GetUserFunc = userManager.GetUserInfo;
        _handler.SessionLoginTimeoutCheckFunc = userManager.SessionLoginTimeoutCheck;
        _handler.HeartHeatCheckFunc = userManager.HeartBeatCheck;
        _handler.ReceivePongFunc = userManager.ReceivePong;
    }

    public void SetRoomDelegate(in RoomManager roomManager)
    {
        _handler.GetRoombyIDFunc = roomManager.GetRoomID;
        _handler.GetRoombyNumberFunc = roomManager.GetRoomNumber;
        _handler.RoomCheckFunc = roomManager.RoomsCheck;
    }

    public void SetMainDelegate(in MainServer server)
    {
        _handler.SendFunc = server.SendData;
        _handler.InnerSendFunc = server.PacketInnerSend;
        _handler.DatabaseSendFunc = server.PacketDatabaseSend;
        _handler.RedisSendFunc = server.PacketRedisSend;
        _handler.GetSessionFunc = server.GetSessionByID;
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
            ServerPacketData data = _msgBuffer.Receive();

            if (_onRecv.TryGetValue(data.PacketType, out var action))
            {
                action(data);
            }
            else
            {
                Logger.Error($"Not found handler : {data.PacketType}");
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