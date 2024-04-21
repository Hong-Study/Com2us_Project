public interface IAuthService
{
    Task<AuthService.LoginResult> LoginAsync(Int64 id, string token);
    // Task<LogoutResponse> Logout(LogoutRequest request);
}