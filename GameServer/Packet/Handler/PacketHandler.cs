namespace GameServer;

public partial class PacketHandler
{
    public Func<string, UserGameData, ErrorCode> AddUserFunc = null!;
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

        // 로그인 처리
        ErrorCode result = AddUserFunc(sessionID, new UserGameData());
        SLoginRes res = new SLoginRes();
        res.ErrorCode = (Int16)result;

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
        res.ErrorCode = (Int16)result;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGOUT);
        SendFunc(sessionID, bytes);
    }
}