public interface IAuthService
{
    Task<AuthService.LoginResult> LoginAsync(string id, string token);
    // Task<LogoutResponse> Logout(LogoutRequest request);
}