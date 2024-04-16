public interface IAuthRepository
{
    Task<bool> CreateUserAsync(UserGameData data);
    // Task<LogoutResponse> Logout(LogoutRequest request);
    Task<bool> CheckUserAsync(int id);
    Task<UserGameData?> GetUserGameDataAsync(int id);
}