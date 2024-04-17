public interface IAuthService
{
    Task<LoginRes> LoginAsync(LoginReq request);
    // Task<LogoutResponse> Logout(LogoutRequest request);
}