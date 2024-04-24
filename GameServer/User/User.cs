namespace GameServer;

public class User
{
    public ClientSession? Session { get; private set; }
    public Int64 UserID { get; set; }
    public int Level { get; set; }
    public string NickName { get; set; } = null!;
    public int exp { get; set; }
    public int Gold { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public string SessionID { get; set; }
    public User(ClientSession session)
    {
        this.Session = session;
        this.SessionID = session.SessionID;
        session.User = this;
    }

    public void Clear()
    {
        if (Session != null)
        {
            Session.User = null;
            Session = null;
        }
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
}