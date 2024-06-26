using Common;
using MemoryPack;

namespace GameServer;

[MemoryPackable]
public partial class RDUserLoginReq : IMessage
{
    public string UserID { get; set; } = null!;
    public string AuthToken { get; set; } = null!;
}

[MemoryPackable]
public partial class RDUserStateSet : IMessage
{
    public string UserID { get; set; } = null!;
    public string State { get; set; } = null!;
}