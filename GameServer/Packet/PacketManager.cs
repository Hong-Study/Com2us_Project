using MemoryPack;
using Common;

namespace GameServer;

public class PacketManager : DataManager
{
    public PacketHandler _handler = new PacketHandler();

    public PacketManager()
    {
        InitHandler();
    }

    public override void InitHandler()
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
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_CONNECTED, _handler.Handle_NTFSessionConnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, Make<NTFSessionDisconnectedReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_SESSION_DISCONNECTED, _handler.Handle_NTFSessionDisconnected);
        _onRecv.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, Make<NTFCheckSessionLoginReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_CHECK_SESSION_LOGIN, _handler.Handle_NTFCheckSessionLogin);
        _onRecv.Add((Int16)InnerPacketType.NTF_HEART_BEAT, Make<NTFHeartBeatReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_HEART_BEAT, _handler.Handle_NTFHeartBeat);
        _onRecv.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, Make<NTFRoomsCheckReq>);
        _onHandler.Add((Int16)InnerPacketType.NTF_ROOMS_CHECK, _handler.Handle_NTFRoomsCheck);
    }

    public void SetUserDelegate(UserManager userManager)
    {
        _handler.AddUserFunc = userManager.AddUser;
        _handler.LoginUserFunc = userManager.LoginUser;
        _handler.RemoveUserFunc = userManager.RemoveUser;
        _handler.GetUserFunc = userManager.GetUserInfo;
        _handler.HeartHeatCheckFunc = userManager.HeartBeatCheck;
        _handler.SessionLoginTimeoutCheckFunc = userManager.SessionLoginTimeoutCheck;
        _handler.ReceivePongFunc = userManager.ReceivePong;
    }

    public void SetRoomDelegate(RoomManager roomManager)
    {
        _handler.GetRoomFunc = roomManager.GetRoom;
        _handler.RoomCheckFunc = roomManager.RoomsCheck;
    }

    public void SetMainDelegate(MainServer server)
    {
        _handler.SendFunc = server.SendData;
    }

    void Make<T>(ServerPacketData data) where T : IMessage, new()
    {
        var packet = MemoryPackSerializer.Deserialize<T>(data.Body);
        if (packet == null)
        {
            return;
        }

        Action<string, IMessage>? action = null;
        if (_onHandler.TryGetValue(data.PacketType, out action))
        {
            action(data.sessionID, packet);
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

        MainServer.MainLogger.Info($"{packetSize} : {type} : {bodyDataSize}");

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
}