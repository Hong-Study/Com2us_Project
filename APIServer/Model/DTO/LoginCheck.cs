public class LoginCheckReq
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
}

public class LoginCheckRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}
