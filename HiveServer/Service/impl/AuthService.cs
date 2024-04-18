public class AuthService : IAuthService
{
    IAuthRepository _authRepo;
    IMemoryRepository _memoryRepo;
    public AuthService(IAuthRepository accountRepo, IMemoryRepository memoryRepo)
    {
        _authRepo = accountRepo;
        _memoryRepo = memoryRepo;
    }

    public record RegisterResut(ErrorCodes ErrorCode, bool IsSuccess);
    public async Task<RegisterResut> RegisterAsync(RegisterReq registerReq)
    {
        bool IsExist = await _authRepo.CheckAccountAsync(registerReq.Email);
        if (IsExist == true)
        {
            return FailedRegister(ErrorCodes.EMAIL_ALREADY_EXISTS);
        }

        string? hashPassword = SHA256Hash.EncodingHash(registerReq.Password);
        if (hashPassword == null)
        {
            return FailedRegister(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        bool isSuccess = await _authRepo.CreateAccountAsync(new UserData
        {
            email = registerReq.Email,
            password = hashPassword
        });

        if (isSuccess == false)
        {
            return FailedRegister(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        // 에러 코드로 처리
        // 굳이 IsSuccess를 사용할 필요가 없다.
        return new RegisterResut(ErrorCodes.NONE, true);
    }

    public record LoginResult(ErrorCodes ErrorCode, int Id, string Token);
    public async Task<LoginResult> LoginAsync(LoginReq loginReq)
    {
        UserData? userData = await _authRepo.GetAccountAsync(loginReq.Email);

        if (userData == null)
        {
            return FailedLogin(ErrorCodes.EMAIL_DOES_NOT_EXIST);
        }

        string? hashPassword = SHA256Hash.EncodingHash(loginReq.Password);
        if (hashPassword == null)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        if (userData.password != hashPassword)
        {
            return FailedLogin(ErrorCodes.PASSWORD_DOES_NOT_MATCH);
        }

        string token = $"{userData.user_id}{DateTime.Now.Ticks}";
        string? hashToken = SHA256Hash.EncodingHash(token);

        if (hashToken == null)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        if (await _memoryRepo.SetAccessToken(userData.user_id, hashToken) == false)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        return new LoginResult(ErrorCodes.NONE, userData.user_id, hashToken);
    }

    public record VerifyLoginResult(ErrorCodes ErrorCode, bool IsSuccess);
    public async Task<VerifyLoginResult> VerifyLoginAsync(VerifyLoginReq VerifyLoginReq)
    {
        string? token = await _memoryRepo.GetAccessToken(VerifyLoginReq.UserId);
        if (token == null)
        {
            return FailedVerifyLogin(ErrorCodes.NOT_LOGIN);
        }

        if (token != VerifyLoginReq.Token)
        {
            return FailedVerifyLogin(ErrorCodes.INVALID_TOKEN);
        }

        return new VerifyLoginResult(ErrorCodes.NONE, true);
    }

    private VerifyLoginResult FailedVerifyLogin(ErrorCodes error)
    {
        return new VerifyLoginResult(error, false);
    }

    private LoginResult FailedLogin(ErrorCodes error)
    {
        return new LoginResult(error, 0, "");
    }

    private RegisterResut FailedRegister(ErrorCodes error)
    {
        return new RegisterResut(error, false);
    }
}