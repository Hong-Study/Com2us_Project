using Common;
namespace GameServer;

public class User
{
    public string SessionID { get; set; } = null!;
    public DateTime ConnectTime { get; set; }
    public bool IsConnect { get; set; } = false;

    public bool IsLogin { get; set; } = false;
    public Int32 RoomID { get; set; } = -1;
    public Int32 RoomNumber { get; set; } = 0;

    public string UserID { get => Data.UserID; set => Data.UserID = value; }
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
        LeaveRoom();
        SessionDisconnect();
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

        IsLogin = true;

        PingTime = DateTime.Now;
    }

    public bool IsConfirm(string sessionID)
    {
        return SessionID == sessionID;
    }

    public void EnterRoom(Int32 roomID, Int32 roomNumber)
    {
        RoomID = roomID;
        RoomNumber = roomNumber;
    }

    public void LeaveRoom()
    {
        RoomID = 0;
        RoomNumber = 0;
    }

    public void SessionConnected(string sessionID)
    {
        IsConnect = true;

        ConnectTime = DateTime.Now;
        SessionID = sessionID;
    }

    public void SessionDisconnect()
    {
        IsConnect = false;
        IsLogin = false;

        ConnectTime = DateTime.MinValue;
        UserID = "";
        SessionID = "";
    }
}