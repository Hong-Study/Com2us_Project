using System.ComponentModel.DataAnnotations;

public class LoginReq
{
    [Required(ErrorMessage = "Id is required")]
    public string UserID { get; set; } = null!;
    
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = null!;
}

public class LoginRes : DefaultRes
{
    public UserGameData? GameData { get; set; }
    public string? GameServerAddress { get; set; }
    public Int32 GameServerPort { get; set; }
}