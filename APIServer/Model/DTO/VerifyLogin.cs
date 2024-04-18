using System.ComponentModel.DataAnnotations;

public class VerifyLoginReq
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
}

public class VerifyLoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}