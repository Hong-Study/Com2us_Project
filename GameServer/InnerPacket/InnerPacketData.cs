using Common;
using MemoryPack;

[MemoryPackable]
public partial class NTFCheckSessionLoginReq : IMessage
{

}

[MemoryPackable]
public partial class NTFHeartBeatReq : IMessage
{

}

[MemoryPackable]
public partial class NTFRoomsCheckReq : IMessage
{

}

[MemoryPackable]
public partial class NTFSessionConnectedReq : IMessage
{
    public string SessionID { get; set; } = null!;
}

[MemoryPackable]
public partial class NTFSessionDisconnectedReq : IMessage
{
    public string SessionID { get; set; } = null!;
}