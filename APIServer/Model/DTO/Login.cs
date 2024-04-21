using System.ComponentModel.DataAnnotations;

public class LoginReq
{
    [Required(ErrorMessage = "Id is required")]
    public Int64 UserId { get; set; }
    
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = null!;
}

public class LoginRes : DefaultRes
{
    public UserGameData? GameData { get; set; }
}