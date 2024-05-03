using Common;
using MemoryPack;

namespace GameServer;

[MemoryPackable]
public partial class DBUserLoginReq : IMessage
{
    public Int64 UserID { get; set; }
}

[MemoryPackable]
public partial class DBUpdateWinLoseCountReq : IMessage
{
    public Int64 UserID { get; set; }
    public Int32 WinCount { get; set; }
    public Int32 LoseCount { get; set; }
}