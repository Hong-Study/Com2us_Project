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

[MemoryPackable]
public partial class NTFUserLoginRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
    public UserData? UserData { get; set; }
}

[MemoryPackable]
public partial class NTFUserLogoutRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

[MemoryPackable]
public partial class NTFUserWinLoseUpdateRes : IResMessage
{
    public ErrorCode ErrorCode { get; set; }
}

// [MemoryPackable]
// public partial class NTFMatchingReq : IMessage
// {
//     public string FirstUserID { get; set; } = null!;
//     public string SecondUserID { get; set; } = null!;
// }