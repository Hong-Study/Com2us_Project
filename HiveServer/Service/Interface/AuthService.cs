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
            return new RegisterRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.EMAIL_ALREADY_EXISTS,
                IsSuccess = false
            };
        }

        System.Console.WriteLine("CreateAccountAsync");

        string? hashPassword = SHA256Hash.EncodingHash(registerReq.Password);
        if (hashPassword == null)
        {
            return new RegisterRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                IsSuccess = false
            };
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
            return new RegisterRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                IsSuccess = false
            };
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
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.EMAIL_DOES_NOT_EXIST,
                Token = ""
            };
        }

        string? hashPassword = SHA256Hash.EncodingHash(loginReq.Password);
        if (hashPassword == null)
        {
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                Token = ""
            };
        }

        // 비밀번호가 일치하는지 체크
        if (userDB.password != hashPassword)
        {
            // StatusCode 수정
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.PASSWORD_DOES_NOT_MATCH,
                Token = ""
            };
        }

        string token = $"{userDB.id}{DateTime.Now.Ticks}";
        string? hashToken = SHA256Hash.EncodingHash(token);

        if (hashToken == null)
        {
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                Token = ""
            };
        }
        
        // 성공
        if (await _memoryRepo.SetAccessToken(userDB.id, hashToken) == false)
        {
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                Token = "",
            };
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
            return new LoginCheckRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.NOT_LOGIN,
                IsSuccess = false
            };
        }

        if (token != loginCheckReq.Token)
        {
            return new LoginCheckRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.INVALID_TOKEN,
                IsSuccess = false
            };
        }

        return new LoginCheckRes()
        {
            IsSuccess = true
        };
    }
}