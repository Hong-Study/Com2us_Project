using Common;

public class CancleMatchingReq
{
    public string UserID { get; set; } = null!;
}

public class CancleMatchingRes
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}