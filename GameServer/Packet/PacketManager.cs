using System.Threading.Tasks.Dataflow;
using MemoryPack;

namespace GameServer;

public class PacketManager
{
    // 패킷 전체 형태까지??
    public static PacketManager Instance { get; set; } = new PacketManager();
    public BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();
    public List<System.Threading.Thread> _logicThreads = new List<System.Threading.Thread>();

    Dictionary<Int16, Action<ClientSession, byte[], Int16>> _onRecv = new Dictionary<Int16, Action<ClientSession, byte[], Int16>>();
    Dictionary<Int16, Action<ClientSession, IMessage>> _onHandler = new Dictionary<Int16, Action<ClientSession, IMessage>>();

    public PacketManager()
    {
        _onRecv.Add((Int16)PacketType.LOGIN, Make<LoginReq>);
        _onHandler.Add((Int16)PacketType.LOGIN, PacketHandler.Handle_C_Login);

        _onRecv.Add((Int16)PacketType.LOGOUT, Make<LogOutReq>);
        _onHandler.Add((Int16)PacketType.LOGOUT, PacketHandler.Handle_C_Logout);

        _onRecv.Add((Int16)PacketType.ROOM_CREATE, Make<RoomCreateReq>);
        _onHandler.Add((Int16)PacketType.ROOM_CREATE, PacketHandler.Handle_C_RoomCreate);

        _onRecv.Add((Int16)PacketType.ROOM_ENTER, Make<RoomEnterReq>);
        _onHandler.Add((Int16)PacketType.ROOM_ENTER, PacketHandler.Handle_C_RoomEnter);

        _onRecv.Add((Int16)PacketType.ROOM_LEAVE, Make<RoomLeaveReq>);
        _onHandler.Add((Int16)PacketType.ROOM_LEAVE, PacketHandler.Handle_C_RoomLeave);

        _onRecv.Add((Int16)PacketType.ROOM_CHAT, Make<RoomChatReq>);
        _onHandler.Add((Int16)PacketType.ROOM_CHAT, PacketHandler.Handle_C_RoomChat);
    }

    public void Distribute(ServerPacketData data)
    {
        _msgBuffer.Post(data);
    }

    public void Start(int threadCount = 1)
    {
        for (int i = 0; i < threadCount; i++)
        {
            System.Threading.Thread thread = new System.Threading.Thread(this.Process);
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
            ServerPacketData data = _msgBuffer.Receive();
            ParsingPacket(data.Session, data.Body, data.PacketType);
        }
    }


    void ParsingPacket(ClientSession session, byte[] buffer, Int16 type)
    {
        // 패킷 만들어서 Queue에 넣어주기
        Action<ClientSession, byte[], Int16>? action = null;
        if (_onRecv.TryGetValue(type, out action))
        {
            action(session, buffer, type);
        }
    }

    void Make<T>(ClientSession session, byte[] bytes, Int16 type) where T : IMessage, new()
    {
        T data = PacketDeserialize<T>(bytes);
        if (_onHandler != null)
        {
            _onHandler[type](session, data);
        }
    }

    T PacketDeserialize<T>(byte[] bytes) where T : IMessage, new()
    {
        T? data = MemoryPackSerializer.Deserialize<T>(bytes);
        if (data == null)
        {
            return new T();
        }

        return data;
    }

    byte[] PacketSerialized<T>(T packet, PacketType type) where T : IMessage
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