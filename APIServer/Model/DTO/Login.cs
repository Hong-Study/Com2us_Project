using System.ComponentModel.DataAnnotations;

public class LoginReq
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = null!;
}

public class LoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
    public UserGameData? GameData { get; set; }
}