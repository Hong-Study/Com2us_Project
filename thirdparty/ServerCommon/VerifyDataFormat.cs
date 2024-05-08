namespace ServerCommon;

public abstract class DefaultRes
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}

public class VerifyLoginReq
{
    public Int64 UserId { get; set; }
    public string Token { get; set; } = null!;
}

public class VerifyLoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}