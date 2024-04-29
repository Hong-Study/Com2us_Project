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

    void Process()
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

    public static ServerPacketData MakeInnerPacket<T>(string sessionID, T packet, Int16 type) where T : IMessage
    {
        byte[] body = MemoryPackSerializer.Serialize(packet);
        return new ServerPacketData(sessionID, body, type);
    }
}