using Common;

namespace GameServer;

public partial class PacketHandler
{
    public Action<string> AddUserFunc = null!;
    public Action<string> RemoveUserFunc = null!;
    public Action<string, ErrorCode, UserData?> LoginUserFunc = null!;
    public Func<string, User?> GetUserFunc = null!;
    public Action SessionLoginTimeoutCheckFunc = null!;
    public Action HeartHeatCheckFunc = null!;
    public Action<string> ReceivePongFunc = null!;

    public Func<Int32, Room?> GetRoomFunc = null!;
    public Action RoomCheckFunc = null!;

    public Func<string, byte[], bool> SendFunc = null!;
    public Action<ServerPacketData> InnerSendFunc = null!;
    public Action<ServerPacketData> DatabaseSendFunc = null!;
    public Action<ServerPacketData> RedisSendFunc = null!;

    public void Handle_C_Login(string sessionID, IMessage message)
    {
        var packet = message as CLoginReq;
        if (packet == null)
        {
            return;
        }

        var req = new RDUserLoginReq();

        req.UserID = packet.UserID;
        req.AuthToken = packet.AuthToken;

        var serverPacketData = RedisManager.MakeRedisPacket(sessionID, req, RedisType.REQ_RD_USER_LOGIN);
        RedisSendFunc(serverPacketData);
    }

    public void Handle_C_Logout(string sessionID, IMessage message)
    {
        var packet = message as CLogOutReq;
        if (packet == null)
        {
            return;
        }

        RemoveUserFunc(sessionID);
    }

    public void Handle_C_Pong(string sessionID, IMessage message)
    {
        var packet = message as CPongRes;
        if (packet == null)
        {
            return;
        }

        ReceivePongFunc(sessionID);
    }

    Room? GetRoom<T>(string sessionID, PacketType packetType) where T : IResMessage, new()
    {
        var user = GetUserFunc(sessionID);
        if (user == null)
        {   
            MainServer.MainLogger.Error($"GetUser : User{sessionID} is not exist");
            
            T pkt = new T();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_USER;

            byte[] bytes = PacketManager.PacketSerialized(pkt, packetType);
            SendFunc(sessionID, bytes);

            return null;
        }

        var room = GetRoomFunc(user.RoomID);
        if (room == null)
        {
            MainServer.MainLogger.Error($"GetRoom({user.UserID}) : Room({user.RoomID}) is not exist");

            T pkt = new T();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_ROOM;

            byte[] bytes = PacketManager.PacketSerialized(pkt, packetType);
            SendFunc(sessionID, bytes);
            
            return null;
        }

        return room;
    }
}