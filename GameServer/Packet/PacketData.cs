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
public partial class LoginReq : IMessage
{
    public Int64 UserID { get; set; }
    public string AuthToken { get; set; } = null!;
}

[MemoryPackable]
public partial class LoginRes : IMessage
{
    public short ErrorCode { get; set; }
}

[MemoryPackable]
public partial class LogOutReq : IMessage
{
    public int Index { get; set; }
}

[MemoryPackable]
public partial class LogOutRes : IMessage
{
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomEnterReq : IMessage
{
    public int RoomNumber { get; set; }
}

[MemoryPackable]
public partial class RoomEnterRes : IMessage
{
    public short ErrorCode { get; set; }
}

[MemoryPackable]
public partial class RoomLeaveReq : IMessage
{
    public int RoomNumber { get; set; }
}

[MemoryPackable]
public partial class RoomLeaveRes : IMessage
{
    public short ErrorCode { get; set; }
}


[MemoryPackable]
public partial class RoomChatReq : IMessage
{
    public string Message { get; set; } = null!;
}

[MemoryPackable]
public partial class RoomChatRes : IMessage
{
    public string UserName { get; set; } = null!;
    public string Message { get; set; } = null!;
}