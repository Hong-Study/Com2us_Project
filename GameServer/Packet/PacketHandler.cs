using Common;

namespace GameServer;

public partial class PacketHandler
{
    public Action<string> AddUserFunc = null!;
    public Action<string> RemoveUserFunc = null!;
    public Action<string> ReceivePongFunc = null!;
    public Action<string, UserGameData> LoginUserFunc = null!;
    public Func<string, User?> GetUserFunc = null!;
    public Action HeartHeatCheckFunc = null!;
    public Action SessionLoginTimeoutCheckFunc = null!;

    public Func<string, byte[], bool> SendFunc = null!;

    public Func<Int32, Room?> GetRoomFunc = null!;
    public Action RoomCheckFunc = null!;

    public void Handle_C_Login(string sessionID, IMessage message)
    {
        var packet = message as CLoginReq;
        if (packet == null)
        {
            return;
        }

        UserGameData data = new UserGameData()
        {
            user_id = packet.UserID,
            user_name = $"User{packet.UserID}",
            win = 0,
            lose = 0,
            level = 1,
        };

        LoginUserFunc(sessionID, data);
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

    Room? GetRoom<T>(string sessionID) where T : IResMessage, new()
    {
        var user = GetUserFunc(sessionID);
        if (user == null)
        {   
            MainServer.MainLogger.Error($"GetUser : User{sessionID} is not exist");
            
            T pkt = new T();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_USER;

            byte[] bytes = PacketManager.PacketSerialized(pkt, PacketType.RES_S_ROOM_ENTER);
            SendFunc(sessionID, bytes);

            return null;
        }

        var room = GetRoomFunc(user.RoomID);
        if (room == null)
        {
            MainServer.MainLogger.Error($"GetRoom : Room({user.UserID}) is not exist");

            T pkt = new T();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_ROOM;

            byte[] bytes = PacketManager.PacketSerialized(pkt, PacketType.RES_S_ROOM_ENTER);
            SendFunc(sessionID, bytes);
            
            return null;
        }

        return room;
    }
}