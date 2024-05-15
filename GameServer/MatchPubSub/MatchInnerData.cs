using Common;
using MemoryPack;

public enum MatchInnerType
{
    NONE = 0,
    MAKE_EMPTY_ROOM = 2,
}

[MemoryPackable]
public partial class MakeEmptyRoomReq : IMessage
{
    public Int32 RoomID { get; set; }
}