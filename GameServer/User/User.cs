using MemoryPack;

namespace GameServer;

public class User
{
    public string SessionID { get; set; } = null!;
    public Int64 UserID { get; set; }
    public int RoomID { get; set; }
    public int Level { get; set; }
    public string NickName { get; set; } = null!;
    public int exp { get; set; }
    public int Gold { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }

    public void Clear()
    {

    }

    public void SetInfo(UserGameData data)
    {
        this.UserID = data.user_id;
        this.NickName = data.user_name;
        this.Level = data.level;
        this.exp = data.exp;
        this.Gold = data.gold;
        this.Win = data.win;
        this.Lose = data.lose;
    }

    public bool IsConfirm(string sessionId)
    {
        return this.SessionID == sessionId;
    }

    public void EnterRoom(int roomId)
    {
        this.RoomID = roomId;
    }

    public void LeaveRoom()
    {
        this.RoomID = -1;
    }
}