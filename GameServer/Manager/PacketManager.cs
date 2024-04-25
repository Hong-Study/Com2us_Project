using System.Threading.Tasks.Dataflow;
using MemoryPack;

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
        _onRecv.Add((Int16)PacketType.LOGIN, Make<LoginReq>);
        _onHandler.Add((Int16)PacketType.LOGIN, _handler.Handle_C_Login);

        _onRecv.Add((Int16)PacketType.LOGOUT, Make<LogOutReq>);
        _onHandler.Add((Int16)PacketType.LOGOUT, _handler.Handle_C_Logout);

        _onRecv.Add((Int16)PacketType.ROOM_ENTER, Make<RoomEnterReq>);
        _onHandler.Add((Int16)PacketType.ROOM_ENTER, _handler.Handle_C_RoomEnter);

        _onRecv.Add((Int16)PacketType.ROOM_LEAVE, Make<RoomLeaveReq>);
        _onHandler.Add((Int16)PacketType.ROOM_LEAVE, _handler.Handle_C_RoomLeave);

        _onRecv.Add((Int16)PacketType.ROOM_CHAT, Make<RoomChatReq>);
        _onHandler.Add((Int16)PacketType.ROOM_CHAT, _handler.Handle_C_RoomChat);
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
        T packet = PacketDeserialize<T>(data.Body);

        Action<string, IMessage>? action = null;
        if (_onHandler.TryGetValue(data.PacketType, out action))
        {
            action(data.SessionID, packet);
        }
    }

    public T PacketDeserialize<T>(byte[] bytes) where T : IMessage, new()
    {
        T? data = MemoryPackSerializer.Deserialize<T>(bytes);
        if (data == null)
        {
            return new T();
        }

        return data;
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