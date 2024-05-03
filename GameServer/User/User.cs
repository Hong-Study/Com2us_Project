using MemoryPack;

namespace GameServer;

public class User
{
    public string sessionID { get; set; } = null!;
    public DateTime ConnectTime { get; set; }
    public bool IsConnect { get; set; } = false;

    public bool IsLogin { get; set; } = false;

    public Int64 UserID { get; set; }
    public Int32 RoomID { get; set; }
    public Int32 Level { get; set; }
    public string NickName { get; set; } = null!;
    public Int32 exp { get; set; }
    public Int32 Gold { get; set; }
    public Int32 Win { get; set; }
    public Int32 Lose { get; set; }

    public DateTime PingTime { get; set; }

    public void Clear()
    {
        Logouted();

        sessionID = "";
        IsConnect = false;
    }

    public void Logined(UserGameData data)
    {
        this.UserID = data.user_id;
        this.NickName = data.user_name;
        this.Level = data.level;
        this.exp = data.exp;
        this.Gold = data.gold;
        this.Win = data.win;
        this.Lose = data.lose;
        this.IsLogin = true;

        PingTime = DateTime.Now;
    }

    public void Logouted()
    {
        this.UserID = 0;
        this.IsLogin = false;
    }

    public bool IsConfirm(string sessionID)
    {
        return this.sessionID == sessionID;
    }

    public void EnterRoom(Int32 roomId)
    {
        this.RoomID = roomId;
    }

    public void LeaveRoom()
    {
        this.RoomID = -1;
    }

    public void SessionConnected(string sessionID)
    {
        this.IsConnect = true;
        this.IsLogin = false;

        this.ConnectTime = DateTime.Now;
        this.sessionID = sessionID;
    }
}