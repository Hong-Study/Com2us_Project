using Common;

namespace GameServer;

public class RoomUser
{
    public string SessionID { get; set; } = null!;
    public Int64 UserID { get; set; }
    public bool IsReady { get; set; } = false;
    public BoardType PlayerColor { get; set; }
    public UserData UserData { get; set; } = null!;
}

public enum BoardType : Int16
{
    None = 0,
    Black = 1,
    White = 2,
}