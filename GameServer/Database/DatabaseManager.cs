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
    IUserRepository userRepository;

    Dictionary<Int16, Action<ServerPacketData>> _onRecv = new Dictionary<Int16, Action<ServerPacketData>>();
    Dictionary<Int16, Func<string, IMessage, Task>> _onHandler = new Dictionary<Int16, Func<string, IMessage, Task>>();
    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    public DatabaseManager(ref readonly ServerOption option)
    {
        userRepository = new UserRepository(option.DatabaseConnectionString);
        _handler = new DatabaseHandler();

        InitHandler();

        SetDelegate();
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

    void SetDelegate()
    {
        _handler.GetUserGameDataAsync = userRepository.GetUserGameDataAsync;
        _handler.UpdateUserWinLoseAsync = userRepository.UpdateUserWinLoseAsync;
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
                    MainServer.MainLogger.Error($"Not found handler : {data.PacketType}");
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

    public static ServerPacketData MakeDatabasePacket<T>(string sessionID, T packet, DatabaseType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}