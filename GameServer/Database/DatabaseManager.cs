using System.Data;
using System.Threading.Tasks.Dataflow;
using Common;
using MemoryPack;
using MySqlConnector;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseManager
{
    DatabaseHandler _handler;
    DBRepository _dbRepository = null!;
    string _connectionString = null!;

    // 어떻게 하면 하나로 묶어서 처리할 수 있을까?
    Dictionary<Int16, Action<ServerPacketData, DBConnector>> _onRecv = new Dictionary<Int16, Action<ServerPacketData, DBConnector>>();
    Dictionary<Int16, Action<string, IMessage, DBConnector>> _onHandler = new Dictionary<Int16, Action<string, IMessage, DBConnector>>();
    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public DatabaseManager(ref readonly ServerOption option)
    {
        _connectionString = option.DatabaseConnectionString;

        _handler = new DatabaseHandler();
        _dbRepository = new DBRepository();

        InitHandler();

        SetDelegate();
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _handler.InitLogger(logger);
    }

    public void InitHandler()
    {
        _onRecv.Add((Int16)DatabaseType.REQ_DB_USER_LOGIN, Make<DBUserLoginReq>);
        _onHandler.Add((Int16)DatabaseType.REQ_DB_USER_LOGIN, _handler.Handle_DB_Login);

        _onRecv.Add((Int16)DatabaseType.REQ_DB_UPDATE_WIN_LOSE_COUNT, Make<DBUpdateWinLoseCountReq>);
        _onHandler.Add((Int16)DatabaseType.REQ_DB_UPDATE_WIN_LOSE_COUNT, _handler.Handle_DB_UpdateWinLoseCount);
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

    public void SetMainServerDelegate(ref readonly MainServer mainServer)
    {
        _handler.InnerSendFunc = mainServer.PacketInnerSend;
    }

    void SetDelegate()
    {
        _handler.GetUserGameDataAsync = _dbRepository.GetUserGameDataAsync;
        _handler.UpdateUserWinLoseAsync = _dbRepository.UpdateUserWinLoseAsync;
    }

    void Process()
    {
        var connector = new DBConnector(_connectionString);

        while (MainServer.IsRunning)
        {
            ServerPacketData data = _msgBuffer.Receive();

            if (_onRecv.TryGetValue(data.PacketType, out var action))
            {
                action(data, connector);
            }
            else
            {
                Logger.Error($"Not found handler : {data.PacketType}");
            }
        }
    }

    void Make<T>(ServerPacketData data, DBConnector connector) where T : IMessage, new()
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

    public static ServerPacketData MakeDatabasePacket<T>(string sessionID, T packet, DatabaseType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}