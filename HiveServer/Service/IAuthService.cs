public interface IAuthService
{
    public Task<RegisterRes> RegisterAsync(RegisterReq registerReq);
    public Task<LoginRes> LoginAsync(LoginReq loginReq);
    public Task<LoginCheckRes> LoginCheckAsync(LoginCheckReq loginCheckReq);
}