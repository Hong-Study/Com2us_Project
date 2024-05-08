public class AuthService : IAuthService
{
    ILogger<AuthService> _logger;
    IAuthRepository _authRepo;
    IMemoryRepository _memoryRepo;
    public AuthService(IAuthRepository accountRepo, IMemoryRepository memoryRepo, ILogger<AuthService> logger)
    {
        _logger = logger;
        _authRepo = accountRepo;
        _memoryRepo = memoryRepo;
    }

    public record RegisterResut(ErrorCode ErrorCode, bool IsSuccess);
    public async Task<RegisterResut> RegisterAsync(RegisterReq registerReq)
    {
        bool IsExist = await _authRepo.CheckAccountAsync(registerReq.Email);
        if (IsExist == true)
        {
            return FailedRegister(ErrorCode.EMAIL_ALREADY_EXISTS);
        }

        string? hashPassword = SHA256Hash.EncodingHash(registerReq.Password);
        if (hashPassword == null)
        {
            return FailedRegister(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        bool isSuccess = await _authRepo.CreateAccountAsync(new UserData
        {
            user_id = registerReq.Email,
            password = hashPassword
        });

        if (isSuccess == false)
        {
            return FailedRegister(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        // 에러 코드로 처리
        // 굳이 IsSuccess를 사용할 필요가 없다.
        return new RegisterResut(ErrorCode.NONE, true);
    }

    public record LoginResult(ErrorCode ErrorCode, string? userID, string? token);
    public async Task<LoginResult> LoginAsync(LoginReq loginReq)
    {
        UserData? userData = await _authRepo.GetAccountAsync(loginReq.Email);
        if (userData == null)
        {
            return FailedLogin(ErrorCode.EMAIL_DOES_NOT_EXIST);
        }

        string? hashPassword = SHA256Hash.EncodingHash(loginReq.Password);
        if (hashPassword == null)
        {
            return FailedLogin(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        if (userData.password != hashPassword)
        {
            return FailedLogin(ErrorCode.PASSWORD_DOES_NOT_MATCH);
        }

        string token = $"{userData.user_id}{DateTime.Now.Ticks}";
        string? hashToken = SHA256Hash.EncodingHash(token);

        if (hashToken == null)
        {
            return FailedLogin(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        if (await _memoryRepo.SetAccessToken(userData.user_id, hashToken) == false)
        {
            return FailedLogin(ErrorCode.INTERNAL_SERVER_ERROR);
        }

        return new LoginResult(ErrorCode.NONE, userData.user_id, hashToken);
    }

    public record VerifyLoginResult(ErrorCode ErrorCode, bool IsSuccess);
    public async Task<VerifyLoginResult> VerifyLoginAsync(VerifyLoginReq VerifyLoginReq)
    {
        string? token = await _memoryRepo.GetAccessToken(VerifyLoginReq.UserID);
        if (token == null)
        {
            return FailedVerifyLogin(ErrorCode.NOT_LOGIN);
        }

        if (token != VerifyLoginReq.Token)
        {
            return FailedVerifyLogin(ErrorCode.INVALID_TOKEN);
        }

        return new VerifyLoginResult(ErrorCode.NONE, true);
    }

    private VerifyLoginResult FailedVerifyLogin(ErrorCode error)
    {
        _logger.LogError("FailedVerifyLogin : " + error.ToString());
        
        return new VerifyLoginResult(error, false);
    }

    private LoginResult FailedLogin(ErrorCode error)
    {
        _logger.LogError("FailedLogin : " + error.ToString());

        return new LoginResult(error, null, null);
    }

    private RegisterResut FailedRegister(ErrorCode error)
    {
        _logger.LogError("FailedRegister : " + error.ToString());

        return new RegisterResut(error, false);

    }
}