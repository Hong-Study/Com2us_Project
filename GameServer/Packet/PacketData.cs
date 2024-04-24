using MemoryPack;

namespace ChattingServer;

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
    [MemoryPackOrder(0)]
    public Int64 UserID { get; set; }
    [MemoryPackOrder(1)]
    public string UserToken { get; set; } = null!;
}

[MemoryPackable]
public partial class LoginRes : IMessage
{
    [MemoryPackOrder(0)]
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class LogOutReq : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class LogOutRes : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomCreateReq : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomCreateRes : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomEnterReq : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomEnterRes : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomLeaveReq : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}

[MemoryPackable]
public partial class RoomLeaveRes : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}


[MemoryPackable]
public partial class RoomChatReq : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}


[MemoryPackable]
public partial class RoomChatRes : IMessage
{
    [MemoryPackOrder(0)]
    public int Index { get; set; }
}