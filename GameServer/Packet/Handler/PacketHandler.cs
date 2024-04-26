namespace GameServer;

public partial class PacketHandler
{
    public Func<string, UserGameData, ErrorCode> AddUserFunc = null!;
    public Func<string, ErrorCode> RemoveUserFunc = null!;
    public Func<string, User?> GetUserFunc = null!;
    public Func<int, Room?> GetRoomFunc = null!;

    public void Handle_C_Login(string sessionID, IMessage message)
    {
        CLoginReq? packet = message as CLoginReq;
        if (packet == null)
        {
            return;
        }

        // 로그인 처리
        AddUserFunc(sessionID, new UserGameData());
    }

    public void Handle_C_Logout(string sessionID, IMessage message)
    {
        CLogOutReq? packet = message as CLogOutReq;
        if (packet == null)
        {
            return;
        }

        // 로그아웃 처리
        RemoveUserFunc(sessionID);
    }
}