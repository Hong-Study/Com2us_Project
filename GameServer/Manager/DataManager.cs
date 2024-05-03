using System.Threading.Tasks.Dataflow;
using Common;
using MemoryPack;

namespace GameServer;

public abstract class DataManager
{
    protected Dictionary<Int16, Action<ServerPacketData>> _onRecv = new Dictionary<Int16, Action<ServerPacketData>>();
    protected Dictionary<Int16, Action<string, IMessage>> _onHandler = new Dictionary<Int16, Action<string, IMessage>>();
    List<Thread> _logicThreads = new List<Thread>();
    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    public abstract void InitHandler();

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

    public static ServerPacketData MakeInnerPacket<T>(string sessionID, T packet, InnerPacketType type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, (Int16)type);
    }
}