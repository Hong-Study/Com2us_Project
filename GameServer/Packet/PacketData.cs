using MemoryPack;

namespace GameServer;

public class PacketDef
{
    public const Int16 PACKET_HEADER_SIZE = 5;
    public const int MAX_USER_ID_BYTE_LENGTH = 16;
    public const int MAX_USER_PW_BYTE_LENGTH = 16;
    public const int INVALID_ROOM_NUMBER = -1;
}

public interface IMessage { }

[MemoryPackable]
public partial class CLoginReq : IMessage
{
    public Int64 UserID { get; set; }
    public string AuthToken { get; set; } = null!;
}
[MemoryPackable]
public partial class SLoginRes : IMessage
{
    public Int16 ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CLogOutReq : IMessage
{
    public int Index { get; set; }
}
[MemoryPackable]
public partial class SLogOutRes : IMessage
{
    public Int16 ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CRoomEnterReq : IMessage
{
    public int RoomNumber { get; set; }
}
[MemoryPackable]
public partial class SRoomEnterRes : IMessage
{
    public Int16 ErrorCode { get; set; }
    [MemoryPackAllowSerialize]
    public List<UserData>? UserList { get; set; }
}

[MemoryPackable]
public partial class SNewUserEnterReq : IMessage
{
    [MemoryPackAllowSerialize]
    public UserData User { get; set; } = null!;
}

[MemoryPackable]
public partial class CRoomLeaveReq : IMessage
{
    public int RoomNumber { get; set; }
}
[MemoryPackable]
public partial class SRoomLeaveRes : IMessage
{
    public Int16 ErrorCode { get; set; }
}
[MemoryPackable]
public partial class SUserLeaveReq: IMessage
{
    public Int64 UserID { get; set; }
}

[MemoryPackable]
public partial class CRoomChatReq : IMessage
{
    public string Message { get; set; } = null!;
}
[MemoryPackable]
public partial class SRoomChatRes : IMessage
{
    public string UserName { get; set; } = null!;
    public string Message { get; set; } = null!;
}

[MemoryPackable]
public partial class CGameReadyReq : IMessage
{
    public int RoomNumber { get; set; }
    public bool IsReady { get; set; }
}
[MemoryPackable]
public partial class SGameReadyRes : IMessage
{
    public Int16 ErrorCode { get; set; }
    public Int64 UserID { get; set; }
    public bool IsReady { get; set; }
}

[MemoryPackable]
public partial class SGameStartReq : IMessage
{
    public int RoomNumber { get; set; }
    public bool IsStart { get; set; }
    public Int64 StartPlayerID { get; set; }
}
[MemoryPackable]
public partial class CGameStartRes : IMessage
{
    public Int16 ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CGamePutReq : IMessage
{
    public int RoomNumber { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
[MemoryPackable]
public partial class SGamePutRes : IMessage
{
    public Int16 ErrorCode { get; set; }
    public Int64 UserID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

[MemoryPackable]
public partial class SGameEndReq : IMessage
{
    public Int64 WinUserID { get; set; }
}
[MemoryPackable]
public partial class CGameEndRes : IMessage
{
    public Int16 ErrorCode { get; set; }
}

[MemoryPackable]
public partial class STurnChangeReq : IMessage
{
    public int RoomNumber { get; set; }
    public Int64 NextTurnUserID { get; set; }
}

[MemoryPackable]
public partial class CTurnChangeRes : IMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class SGameCancleReq : IMessage
{
    public int RoomID { get; set; }
    public bool IsCancle { get; set; }
}
[MemoryPackable]
public partial class CGameCancleRes : IMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class UserData
{
    public Int64 UserID { get; set; }
    public Int16 PlayerColor { get; set; }
    public string NickName { get; set; } = null!;
    public int Level { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
}