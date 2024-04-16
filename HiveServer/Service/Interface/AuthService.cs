public class AuthService : IAuthService
{
    IAuthRepository _authRepo;
    IMemoryRepository _memoryRepo;
    public AuthService(IAuthRepository accountRepo, IMemoryRepository memoryRepo)
    {
        _authRepo = accountRepo;
        _memoryRepo = memoryRepo;
    }

    public async Task<RegisterRes> RegisterAsync(RegisterReq registerReq)
    {
        // DB에 존재하는지 체크
        // _authRepo.GetAccountAsync(email);
        bool IsExist = await _authRepo.CheckAccountAsync(registerReq.Email);
        if (IsExist == true)
        {
            // StatusCode 수정
            return FailedRegister(ErrorCodes.EMAIL_ALREADY_EXISTS);
        }

        string? hashPassword = SHA256Hash.EncodingHash(registerReq.Password);
        if (hashPassword == null)
        {
            return FailedRegister(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        // 없다면 계정 생성
        bool isSuccess = await _authRepo.CreateAccountAsync(new UserDB
        {
            email = registerReq.Email,
            password = hashPassword
        });

        // 있다면 실패 처리
        if (isSuccess == false)
        {
            // StatusCode 수정
            return FailedRegister(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        // 성공
        return new RegisterRes()
        {
            StatusCode = 200,
            ErrorCode = ErrorCodes.NONE,
            IsSuccess = isSuccess
        };
    }

    public async Task<LoginRes> LoginAsync(LoginReq loginReq)
    {
        // DB에 존재하는지 체크
        UserDB? userDB = await _authRepo.GetAccountAsync(loginReq.Email);

        // 없다면 실패 처리
        if (userDB == null)
        {
            // StatusCode 수정
            return FailedLogin(ErrorCodes.EMAIL_DOES_NOT_EXIST);
        }

        string? hashPassword = SHA256Hash.EncodingHash(loginReq.Password);
        if (hashPassword == null)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        // 비밀번호가 일치하는지 체크
        if (userDB.password != hashPassword)
        {
            // StatusCode 수정
            return FailedLogin(ErrorCodes.PASSWORD_DOES_NOT_MATCH);
        }

        string token = $"{userDB.id}{DateTime.Now.Ticks}";
        string? hashToken = SHA256Hash.EncodingHash(token);

        if (hashToken == null)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }
        
        // 성공
        if (await _memoryRepo.SetAccessToken(userDB.id, hashToken) == false)
        {
            return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
        }

        return new LoginRes()
        {
            StatusCode = 200,
            ErrorCode = ErrorCodes.NONE,
            Id = userDB.id,
            Token = hashToken,
        };
    }

    public async Task<LoginCheckRes> LoginCheckAsync(LoginCheckReq loginCheckReq)
    {
        string? token = await _memoryRepo.GetAccessToken(loginCheckReq.Id);
        System.Console.WriteLine("LoginCheckAsync " + token);
        if (token == null)
        {
            return FailedLoginCheck(ErrorCodes.NOT_LOGIN);
        }

        if (token != loginCheckReq.Token)
        {
            return FailedLoginCheck(ErrorCodes.INVALID_TOKEN);
        }

        return new LoginCheckRes()
        {
            StatusCode = 200,
            ErrorCode = ErrorCodes.NONE,
            IsSuccess = true
        };
    }

    private LoginCheckRes FailedLoginCheck(ErrorCodes error)
    {
        return new LoginCheckRes()
        {
            StatusCode = 400,
            ErrorCode = error,
            IsSuccess = false
        };
    }

    private LoginRes FailedLogin(ErrorCodes error)
    {
        return new LoginRes()
        {
            StatusCode = 400,
            ErrorCode = error,
            Token = ""
        };
    }

    private RegisterRes FailedRegister(ErrorCodes error)
    {
        return new RegisterRes()
        {
            StatusCode = 400,
            ErrorCode = error,
            IsSuccess = false
        };
    }
}