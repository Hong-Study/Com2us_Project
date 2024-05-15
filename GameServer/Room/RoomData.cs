namespace GameServer;

public class RoomUser
{
    public string SessionID { get; set; } = null!;
    public string UserID { get; set; } = null!;
    public bool IsReady { get; set; } = false;
    public BoardType PlayerColor { get; set; }
    public Int16 TimeoutCount { get; set; } = 0;
}

public enum BoardType : Int16
{
    None = 0,
    Black = 1,
    White = 2,
}

public class UsingRoomInfo
{
    public Int32 RoomID { get; set; }
    public RoomState RoomState { get; set; } = RoomState.Empty;
}

public enum RoomState
{
    NONE = 0,
    Empty,
    Waiting,
    Mathcing
}