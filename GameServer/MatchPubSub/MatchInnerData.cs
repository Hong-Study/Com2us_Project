using Common;
using MemoryPack;

public enum MatchInnerType
{
    NONE = 0,

    MATCH_PACKET_START = 5000,

    MAKE_EMPTY_ROOM = 5001,

    MATCH_PACKET_END = 5999,
}

[MemoryPackable]
public partial class MakeEmptyRoomReq : IMessage
{
    public Int32 RoomID { get; set; }
}