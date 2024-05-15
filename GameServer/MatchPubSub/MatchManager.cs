using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using CloudStructures.Structures;
using Common;
using MemoryPack;
using StackExchange.Redis;

namespace GameServer;

public class MatchManager
{
    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    MatchRedisRepository _matchRedisRepository = null!;

    Task _logicTask = null!;

    BufferBlock<ServerPacketData> _msgBuffer = new BufferBlock<ServerPacketData>();

    public MatchManager(ref readonly ServerOption option)
    {
        _matchRedisRepository = new MatchRedisRepository("127.0.0.1", option.Port, option.RedisConnectionString
                                                        , option.RedisSubKey, option.RedisPubKey);
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _matchRedisRepository.InitLogger(logger);
    }

    public void SetMainDelegate(ref readonly MainServer mainServer)
    {
        _matchRedisRepository.SetInnerFunc(mainServer.PacketInnerSend);
    }

    public void InitUsingRoomList(ref readonly RoomManager roomManager)
    {
        _matchRedisRepository.InitUsingRoomList(in roomManager);
    }

    public void Distribute(ServerPacketData message)
    {
        _msgBuffer.Post(message);
    }

    public void Start()
    {
        _logicTask = Process();
    }

    public void Stop()
    {
        _logicTask.Wait();
    }

    async Task Process()
    {
        TimeSpan timeout = TimeSpan.FromMilliseconds(100);

        while (MainServer.IsRunning)
        {
            try
            {
                await _matchRedisRepository.Process();

                ServerPacketData message = _msgBuffer.Receive(timeout);
                if (message.PacketType == (Int16)MatchInnerType.MAKE_EMPTY_ROOM)
                {
                    var data = MemoryPackSerializer.Deserialize<MakeEmptyRoomReq>(message.Body);
                    if (data == null)
                    {
                        Logger.Error("MakeEmptyRoomReq Deserialize Fail");
                        continue;
                    }

                    _matchRedisRepository.SetEmptyRoom(data.RoomID);
                }
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

    public static ServerPacketData MakeInnerPacket<T>(MatchInnerType type, T message)
    {
        byte[] body = MemoryPackSerializer.Serialize(message);
        return new ServerPacketData("", body, (Int16)type);
    }
}