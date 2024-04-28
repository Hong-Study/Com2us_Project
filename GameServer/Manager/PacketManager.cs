using System.Threading.Tasks.Dataflow;
using MemoryPack;
using Common;

namespace GameServer;

public class PacketManager
{
    // 패킷 전체 형태까지??
    public PacketHandler _handler = new PacketHandler();

    public BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();
    public List<Thread> _logicThreads = new List<Thread>();

    Dictionary<Int16, Action<ServerPacketData>> _onRecv = new Dictionary<Int16, Action<ServerPacketData>>();
    Dictionary<Int16, Action<string, IMessage>> _onHandler = new Dictionary<Int16, Action<string, IMessage>>();

    public PacketManager()
    {
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
    }

    public void InitUserDelegate(UserManager userManager)
    {
        _handler.AddUserFunc = userManager.AddUser;
        _handler.RemoveUserFunc = userManager.RemoveUser;
        _handler.GetUserFunc = userManager.GetUserInfo;
    }

    public void InitRoomDelegate(RoomManager roomManager)
    {
        _handler.GetRoomFunc = roomManager.GetRoom;
    }

    public void InitSendFunc(Func<string, byte[], bool> sendFunc)
    {
        _handler.SendFunc = sendFunc;
    }

    public void Distribute(ServerPacketData data)
    {
        _msgBuffer.Post(data);
    }

    public void Start(int threadCount = 1)
    {
        for (int i = 0; i < threadCount; i++)
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

    public void Process()
    {
        while (MainServer.IsRunning)
        {
            // 멈출 때, Blocking 처리를 어떻게 할 지 고민해야 함.
            try
            {
                TimeSpan timeOut = TimeSpan.FromMilliseconds(1000);
                ServerPacketData data = _msgBuffer.Receive(timeOut);

                Action<ServerPacketData>? action = null;
                if (_onRecv.TryGetValue(data.PacketType, out action))
                {
                    action(data);
                }
            }
            catch
            {

            }
        }
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
            action(data.SessionID, packet);
        }
    }

    public static byte[] PacketSerialized<T>(T packet, PacketType type) where T : IMessage
    {
        byte[]? bodyData = MemoryPackSerializer.Serialize(packet);
        Int16 bodyDataSize = 0;
        if (bodyData != null)
        {
            bodyDataSize = (Int16)bodyData.Length;
        }

        var packetSize = (Int16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

        var dataSource = new byte[packetSize];
        Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes((Int16)type), 0, dataSource, 2, 2);
        dataSource[4] = 0;

        if (bodyData != null)
        {
            Buffer.BlockCopy(bodyData, 0, dataSource, PacketDef.PACKET_HEADER_SIZE, bodyData.Length);
        }

        return dataSource;
    }
}