using Common;
using MemoryPack;

namespace GameServer;

[MemoryPackable]
public partial class MEUserLoginReq : IMessage
{
    public Int64 UserID { get; set; }
    public string AuthToken { get; set; } = null!;
}