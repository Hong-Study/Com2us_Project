using System.Threading.Tasks.Dataflow;
using CloudStructures;
using Common;
using MemoryPack;

namespace GameServer;

public class RedisManager
{
    RedisHandler _handler;
    RedisRepository _redis = null!;
    string _connectionString = null!;

    Dictionary<Int16, Action<ServerPacketData, RedisConnector>> _onRecv = new Dictionary<Int16, Action<ServerPacketData, RedisConnector>>();
    Dictionary<Int16, Action<string, IMessage, RedisConnector>> _onHandler = new Dictionary<Int16, Action<string, IMessage, RedisConnector>>();

    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _handler.InitLogger(logger);
        _redis.InitLogger(logger);
    }

    public RedisManager(ref readonly ServerOption option)
    {
        _connectionString = option.RedisConnectionString;

        _handler = new RedisHandler();
        _redis = new RedisRepository();

        InitHandler();

        SetDelegate();
    }

    public void InitHandler()
    {
        _onRecv.Add((Int16)RedisType.REQ_RD_USER_LOGIN, Make<RDUserLoginReq>);
        _onHandler.Add((Int16)RedisType.REQ_RD_USER_LOGIN, _handler.Handle_RD_UserLogin);

        _onRecv.Add((Int16)RedisType.SET_RD_USER_STATE, Make<RDUserStateSet>);
        _onHandler.Add((Int16)RedisType.SET_RD_USER_STATE, _handler.Handle_RD_SetUserState);
    }

    public void SetMainServerDelegate(ref readonly MainServer mainServer)
    {
        _handler.InnerSendFunc = mainServer.PacketInnerSend;
        _handler.DatabaseSendFunc = mainServer.PacketDatabaseSend;
    }

    void SetDelegate()
    {
        _handler.ValidataeTokenFunc = _redis.ValidateToken;
        _handler.SetUserStateFunc = _redis.SetUserState;
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
        var redisConnector = new RedisConnector(_connectionString);

        while (MainServer.IsRunning)
        {
            ServerPacketData data = _msgBuffer.Receive();

            if (_onRecv.TryGetValue(data.PacketType, out var action))
            {
                action(data, redisConnector);
            }
            else
            {
                Logger.Error($"Not found handler : {data.PacketType}");
            }
        }
    }

    void Make<T>(ServerPacketData data, RedisConnector connector) where T : IMessage, new()
    {
        var packet = MemoryPackSerializer.Deserialize<T>(data.Body);
        if (packet == null)
        {
            return;
        }

        if (_onHandler.TryGetValue(data.PacketType, out var action))
        {
            action(data.sessionID, packet, connector);
        }
    }

    public static ServerPacketData MakeRedisPacket<T>(string sessionID, T packet, RedisType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}