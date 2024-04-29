using Common;

namespace GameServer;

public partial class PacketHandler
{
    public Func<string, UserGameData, UserManager.AddUserResult> AddUserFunc = null!;
    public Func<string, ErrorCode> RemoveUserFunc = null!;
    public Func<string, User?> GetUserFunc = null!;
    public Func<int, Room?> GetRoomFunc = null!;
    public Func<string, byte[], bool> SendFunc = null!;

    public void Handle_C_Login(string sessionID, IMessage message)
    {
        CLoginReq? packet = message as CLoginReq;
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

        // 로그인 처리
        UserManager.AddUserResult result = AddUserFunc(sessionID, data);
        SLoginRes res = new SLoginRes();
        res.ErrorCode = result.ErrorCode;

        if (result.data == null)
        {
            return;
        }

        MainServer.MainLogger.Debug($"User Login : {result.data.user_id}");

        res.UserData = new UserData()
        {
            UserID = result.data.user_id,
            PlayerColor = (Int16)BoardType.None,
            NickName = result.data.user_name,
            Win = result.data.win,
            Lose = result.data.lose,
            Level = result.data.level,
        };

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionID, bytes);
    }

    public void Handle_C_Logout(string sessionID, IMessage message)
    {
        CLogOutReq? packet = message as CLogOutReq;
        if (packet == null)
        {
            return;
        }

        // 로그아웃 처리
        ErrorCode result = RemoveUserFunc(sessionID);
        SLogOutRes res = new SLogOutRes();
        res.ErrorCode = result;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGOUT);
        SendFunc(sessionID, bytes);
    }

    Room? GetRoom(string sessionID)
    {
        var user = GetUserFunc(sessionID);
        if (user == null)
        {
            return null;
        }

        return GetRoomFunc(user.RoomID);
    }
}