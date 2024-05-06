using Common;
namespace GameServer;

public class User
{
    public string SessionID { get; set; } = null!;
    public DateTime ConnectTime { get; set; }
    public bool IsConnect { get; set; } = false;

    public bool IsLogin { get; set; } = false;
    public Int32 RoomID { get; set; } = 0;

    public Int64 UserID { get => Data.UserID; set => Data.UserID = value; }
    public Int32 Level { get => Data.Level; set => Data.Level = value; }
    public string NickName { get => Data.NickName; set => Data.NickName = value; }
    public Int32 Exp { get => Data.Exp; set => Data.Exp = value; }
    public Int32 Gold { get => Data.Gold; set => Data.Gold = value; }
    public Int32 Win { get => Data.Win; set => Data.Win = value; }
    public Int32 Lose { get => Data.Lose; set => Data.Lose = value; }

    public DateTime PingTime { get; set; }

    public UserData Data { get; set; } = new UserData();

    public void Clear()
    {
        Logouted();

        SessionID = "";
        RoomID = 0;
        IsConnect = false;
    }

    public void Logined(UserData data)
    {
        UserID = data.UserID;
        Level = data.Level;
        NickName = data.NickName;
        Exp = data.Exp;
        Gold = data.Gold;
        Win = data.Win;
        Lose = data.Lose;
        
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
        return this.SessionID == sessionID;
    }

    public void EnterRoom(Int32 roomId)
    {
        this.RoomID = roomId;
    }

    public void LeaveRoom()
    {
        this.RoomID = 0;
    }

    public void SessionConnected(string sessionID)
    {
        this.IsConnect = true;
        this.IsLogin = false;

        this.ConnectTime = DateTime.Now;
        this.SessionID = sessionID;
    }
}