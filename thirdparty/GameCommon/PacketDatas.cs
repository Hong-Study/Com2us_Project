using System;
using System.Collections.Generic;
using MemoryPack;

namespace Common;

public class PacketDef
{
    public const Int16 PACKET_HEADER_SIZE = 5;
    public const Int16 MAX_USER_ID_BYTE_LENGTH = 16;
    public const Int16 MAX_USER_PW_BYTE_LENGTH = 16;
    public const Int16 INVALID_ROOM_NUMBER = -1;
}

public interface IMessage { }
public interface IResMessage : IMessage 
{ 
    ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class SPingReq : IMessage
{

}

[MemoryPackable]
public partial class CPongRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class SConnectedRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CLoginReq : IMessage
{
    public Int64 UserID { get; set; }
    public string AuthToken { get; set; } = null!;
}
[MemoryPackable]
public partial class SLoginRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CLogOutReq : IMessage
{
    public Int32 Index { get; set; }
}
[MemoryPackable]
public partial class SLogOutRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CRoomEnterReq : IMessage
{
    public Int32 RoomNumber { get; set; }
}
[MemoryPackable]
public partial class SRoomEnterRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
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
    public Int32 RoomNumber { get; set; }
}
[MemoryPackable]
public partial class SRoomLeaveRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}
[MemoryPackable]
public partial class SUserLeaveReq : IMessage
{
    public Int64 UserID { get; set; }
}

[MemoryPackable]
public partial class CRoomChatReq : IMessage
{
    public string Message { get; set; } = null!;
}
[MemoryPackable]
public partial class SRoomChatRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
    public string? UserName { get; set; }
    public string? Message { get; set; }
}

[MemoryPackable]
public partial class CGameReadyReq : IMessage
{
    public bool IsReady { get; set; }
}
[MemoryPackable]
public partial class SGameReadyRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
    public Int64 UserID { get; set; }
    public bool IsReady { get; set; }
}

[MemoryPackable]
public partial class SGameStartReq : IMessage
{
    public bool IsStart { get; set; }
    public Int64 StartPlayerID { get; set; }
}
[MemoryPackable]
public partial class CGameStartRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class CGamePutReq : IMessage
{
    public Int32 X { get; set; }
    public Int32 Y { get; set; }
}
[MemoryPackable]
public partial class SGamePutRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
    public Int64 UserID { get; set; }
    public Int32 PosX { get; set; }
    public Int32 PosY { get; set; }
}

[MemoryPackable]
public partial class SGameEndReq : IMessage
{
    public Int64 WinUserID { get; set; }
}
[MemoryPackable]
public partial class CGameEndRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class STurnChangeReq : IMessage
{
    public Int64 NextTurnUserID { get; set; }
}
[MemoryPackable]
public partial class CTurnChangeRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class SGameCancleReq : IMessage
{
    public bool IsCancle { get; set; }
}
[MemoryPackable]
public partial class CGameCancleRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class UserData
{
    public Int64 UserID { get; set; }
    public Int16 PlayerColor { get; set; }
    public string NickName { get; set; } = null!;
    public Int32 Level { get; set; }
    public Int32 Win { get; set; }
    public Int32 Lose { get; set; }
}