public interface IAuthService
{
    Task<AuthService.LoginResult> LoginAsync(LoginReq request);
    // Task<LogoutResponse> Logout(LogoutRequest request);
}