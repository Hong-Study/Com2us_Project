using System.Net;

public class AuthService : IAuthService
{
    IAuthRepository _authRepo;
    IMemoryRepository _memoryRepo;
    IConfiguration _config;
    HttpClient _client = new HttpClient();

    public AuthService(IConfiguration config
                        , IAuthRepository authRepository
                        , IMemoryRepository memoryRepository)
    {
        _config = config;
        _authRepo = authRepository;
        _memoryRepo = memoryRepository;
    }

    public record LoginResult(ErrorCode errorCode, UserGameData? gameData, string? gameServerAddress = null, Int32 gameServerPort = 0);
    public async Task<LoginResult> LoginAsync(Int64 id, string token)
    {
        string? url = _config.GetValue<string>("HiveServerUrl");
        if (url == null)
        {
            return FailedLogin(ErrorCode.FAILED_LOGIN);
        }

        _client.BaseAddress = new Uri(url);

        try
        {
            VerifyLoginReq verifyLoginReq = new VerifyLoginReq
            {
                UserId = id,
                Token = token
            };

            HttpResponseMessage resopnse = await _client.PostAsJsonAsync("api/verifylogin", verifyLoginReq);
            if (resopnse.StatusCode != HttpStatusCode.OK)
            {
                return FailedLogin(ErrorCode.FAILED_VERIFY_LOGIN);
            }

            VerifyLoginRes? VerifyLoginRes = await resopnse.Content.ReadFromJsonAsync<VerifyLoginRes>();
            if (VerifyLoginRes == null)
            {
                return FailedLogin(ErrorCode.FAILED_VERIFY_LOGIN_PARSING);
            }

            if (!VerifyLoginRes.IsSuccess)
            {
                return FailedLogin(ErrorCode.FAILED_VERIFY_LOGIN);
            }

            bool IsSuccess = await _memoryRepo.SetAccessToken(id.ToString(), token);
            if (!IsSuccess)
            {
                return FailedLogin(ErrorCode.FAILED_SET_TOKEN);
            }

            UserGameData? gameData = await GetOrCreateUserGameData(id);
            if (gameData == null)
            {
                await _memoryRepo.DeleteAccessToken(id.ToString());
                return FailedLogin(ErrorCode.ERROR_USER_GAME_DATA);
            }

            return new LoginResult(ErrorCode.NONE, gameData, "127.0.0.1", 7777);

        }
        catch(Exception e)
        {
            System.Console.WriteLine($"Failed Hive Connect {e.Message}");
            return FailedLogin(ErrorCode.FAILED_HIVE_CONNECT);
        }
    }

    private LoginResult FailedLogin(ErrorCode errorCode)
    {
        return new LoginResult(errorCode, null);
    }

    // 함수는 하나의 일만 하도록 수정하기
    private async Task<UserGameData?> GetOrCreateUserGameData(Int64 id)
    {
        bool isExist = await _authRepo.CheckUserAsync(id);
        if (!isExist)
        {
            if(await CreateUserGameData(id) == false)
            {
                System.Console.WriteLine("Create User Game Data Failed");
                return null;
            }
        }
        return await _authRepo.GetUserGameDataAsync(id);
    }

    private async Task<bool> CreateUserGameData(Int64 id)
    {
        return await _authRepo.CreateUserAsync(new UserGameData
        {
            user_id = id,
            user_name = "user_" + id,
            gold = 0,
            level = 1,
            exp = 0,
            win = 0,
            lose = 0
        });
    }
}