
public class VerifyLoginReq
{
    public Int64 UserId { get; set; }
    public string Token { get; set; } = null!;
}

public class VerifyLoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}