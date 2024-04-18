using System.ComponentModel.DataAnnotations;

public class LoginCheckReq
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = null!;
}

public class LoginCheckRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}