public class VerifyLoginReq
{
    public string UserID { get; set; } = null!;
    public string Token { get; set; } = null!;
}

public class VerifyLoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}