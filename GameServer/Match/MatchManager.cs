using System.Collections.Concurrent;
using MemoryPack;

namespace GameServer;

public class MatchManager
{
    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    MatchWorker _matchWorker = null!;

    Thread _logicThread = null!;

    ConcurrentQueue<byte[]> _msgQueue = new ConcurrentQueue<byte[]>();

    public MatchManager(ref readonly ServerOption option)
    {
        _matchWorker = new MatchWorker("127.0.0.1", option.Port, option.RedisConnectionString
                                                        , option.RedisSubKey, option.RedisPubKey);
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _matchWorker.InitLogger(logger);
    }

    public void SetMainDelegate(ref readonly MainServer mainServer)
    {
        _matchWorker.SetInnerFunc(mainServer.PacketInnerSend);
    }

    public void InitUsingRoomList(ref readonly RoomManager roomManager)
    {
        _matchWorker.InitUsingRoomList(in roomManager);
    }

    public void Distribute(byte[] message)
    {
        _msgQueue.Enqueue(message);
    }

    public void Start()
    {
        _logicThread = new Thread(Process);
        _logicThread.Start();
    }

    public void Stop()
    {
        _logicThread.Join();
    }

    void Process()
    {
        TimeSpan timeout = TimeSpan.FromMilliseconds(100);

        while (MainServer.IsRunning)
        {
            try
            {
                _matchWorker.Process();

                if (_msgQueue.TryDequeue(out var message) == false)
                {
                    Thread.Sleep(1);
                    continue;
                }

                var data = MemoryPackSerializer.Deserialize<MakeEmptyRoomReq>(message);
                if (data == null)
                {
                    Logger.Error("MakeEmptyRoomReq Deserialize Fail");
                    continue;
                }

                _matchWorker.SetEmptyRoom(data.RoomNumber);
            }
            catch (TimeoutException)
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}