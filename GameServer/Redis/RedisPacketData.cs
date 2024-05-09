using Common;
using MemoryPack;

namespace GameServer;

[MemoryPackable]
public partial class RDUserLoginReq : IMessage
{
    public string UserID { get; set; } = null!;
    public string AuthToken { get; set; } = null!;
}