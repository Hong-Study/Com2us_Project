using System.Threading.Tasks.Dataflow;
using CloudStructures;
using Common;
using MemoryPack;

namespace GameServer;

public class RedisManager
{
    // 스레드 별로 커넥턴을 가지고 있는 것이 좋다.
    RedisHandler _handler;
    
    Dictionary<Int16, Action<ServerPacketData>> _onRecv = new Dictionary<Int16, Action<ServerPacketData>>();
    Dictionary<Int16, Func<string, IMessage, Task>> _onHandler = new Dictionary<Int16, Func<string, IMessage, Task>>();
    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    RedisRepository _redis = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _handler.InitLogger(logger);
        _redis.InitLogger(logger);
    }

    public RedisManager(ref readonly ServerOption option)
    {
        _handler = new RedisHandler();
        _redis = new RedisRepository(option.RedisConnectionString);

        InitHandler();

        SetDelegate();
    }

    public void InitHandler()
    {
        _onRecv.Add((Int16)RedisType.REQ_RD_USER_LOGIN, Make<RDUserLoginReq>);
        _onHandler.Add((Int16)RedisType.REQ_RD_USER_LOGIN, _handler.Handle_RD_UserLogin);
    }

    public void SetMainServerDelegate(ref readonly MainServer mainServer)
    {
        _handler.InnerSendFunc = mainServer.PacketInnerSend;
        _handler.DatabaseSendFunc = mainServer.PacketDatabaseSend;
    }

    void SetDelegate()
    {
        _handler.ValidataeTokenFunc = _redis.ValidateToken;
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

    public void Distribute(ServerPacketData data)
    {
        _msgBuffer.Post(data);
    }

    void Process()
    {
        while (MainServer.IsRunning)
        {
            // 멈출 때, Blocking 처리를 어떻게 할 지 고민해야 함.
            try
            {
                TimeSpan timeOut = TimeSpan.FromSeconds(1);
                ServerPacketData data = _msgBuffer.Receive(timeOut);

                Action<ServerPacketData>? action = null;
                if (_onRecv.TryGetValue(data.PacketType, out action))
                {
                    action(data);
                }
                else
                {
                    Logger.Error($"Not found handler : {data.PacketType}");
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

        Func<string, IMessage, Task>? action = null;
        if (_onHandler.TryGetValue(data.PacketType, out action))
        {
            action(data.sessionID, packet);
        }
    }

    public static ServerPacketData MakeRedisPacket<T>(string sessionID, T packet, RedisType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}