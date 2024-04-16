public class LoginReq
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
}

public class LoginRes : DefaultRes
{
    public bool IsSuccess { get; set; }
    public UserGameData? GameData { get; set; }

}