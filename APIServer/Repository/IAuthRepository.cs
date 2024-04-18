public interface IAuthRepository
{
    Task<bool> CreateUserAsync(UserGameData data);
    // Task<LogoutResponse> Logout(LogoutRequest request);
    Task<bool> CheckUserAsync(int userId);
    Task<UserGameData?> GetUserGameDataAsync(int userId);
}