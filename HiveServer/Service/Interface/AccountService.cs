public class AccountService : IAccountService
{
    IAccountRepository _accountRepo;
    IMemoryRepository _memoryRepo;
    public AccountService(IAccountRepository accountRepo, IMemoryRepository memoryRepo)
    {
        _accountRepo = accountRepo;
        _memoryRepo = memoryRepo;
    }

    public async Task<RegisterRes> CreateAccountAsync(RegisterReq registerReq)
    {
        // DB에 존재하는지 체크
        // _accountRepo.GetAccountAsync(email);
        bool IsExist = await _accountRepo.CheckAccountAsync(registerReq.Email);
        if(IsExist == true)
        {
            // StatusCode 수정
            return new RegisterRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.EMAIL_ALREADY_EXISTS,
                IsSuccess = false
            };
        }

        System.Console.WriteLine("CreateAccountAsync");

        // 없다면 계정 생성
        bool isSuccess = await _accountRepo.CreateAccountAsync(new UserDB
        {
            email = registerReq.Email,
            password = registerReq.Password
        });

        // 있다면 실패 처리
        if (isSuccess == false)
        {
            // StatusCode 수정
            return new RegisterRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                IsSuccess = false
            };
        }

        // 성공
        return new RegisterRes()
        {
            StatusCode = 200,
            ErrorCode = ErrorCodeEnum.NONE,
            IsSuccess = isSuccess
        };
    }

    public async Task<LoginRes> LoginAsync(LoginReq loginReq)
    {
        // DB에 존재하는지 체크
        UserDB? userDB = await _accountRepo.GetAccountAsync(loginReq.Email);

        // 없다면 실패 처리
        if (userDB == null)
        {
            // StatusCode 수정
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.EMAIL_DOES_NOT_EXIST,
                Token = ""
            };
        }

        // 비밀번호가 일치하는지 체크
        if (userDB.password != loginReq.Password)
        {
            // StatusCode 수정
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.PASSWORD_DOES_NOT_MATCH,
                Token = ""
            };
        }

        string token = "Hello";

        // 성공
        if (await _memoryRepo.SetAccessToken(userDB.id, token) == false)
        {
            return new LoginRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.INTERNAL_SERVER_ERROR,
                Token = "",
            };
        }

        return new LoginRes()
        {
            Token = token
        };
    }

    public async Task<LoginCheckRes> LoginCheckAsync(LoginCheckReq loginCheckReq)
    {
        string? token = await _memoryRepo.GetAccessToken(loginCheckReq.UserId);

        if(token == null)
        {
            return new LoginCheckRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.NOT_LOGIN,
                IsSuccess = false
            };
        }

        if(token != loginCheckReq.Token)
        {
            return new LoginCheckRes()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodeEnum.INVALID_TOKEN,
                IsSuccess = false
            };
        }

        return new LoginCheckRes()
        {
            IsSuccess = true
        };
    }
}