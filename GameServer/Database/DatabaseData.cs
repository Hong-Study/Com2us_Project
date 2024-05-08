using Common;
using MemoryPack;

namespace GameServer;

[MemoryPackable]
public partial class DBUserLoginReq : IMessage
{
    public string UserID { get; set; } = null!;
}

[MemoryPackable]
public partial class DBUpdateWinLoseCountReq : IMessage
{
    public string UserID { get; set; } = null!;
    public Int32 WinCount { get; set; }
    public Int32 LoseCount { get; set; }
}