public interface IAuthService
{
    public Task<AuthService.RegisterResut> RegisterAsync(RegisterReq registerReq);
    public Task<AuthService.LoginResult> LoginAsync(LoginReq loginReq);
    public Task<AuthService.VerifyLoginResult> VerifyLoginAsync(VerifyLoginReq VerifyLoginReq);
}