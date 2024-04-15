public class LoginCheckReq
{
    public long UserId { get; set; }
    public string Token { get; set; } = null!;
}

public class LoginCheckRes : DefaultResponse
{
    public bool IsSuccess { get; set; }
}
