using System.Globalization;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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

    public record LoginResult(ErrorCodes errorCode, UserGameData? gameData);
    public async Task<LoginResult> LoginAsync(Int64 id, string token)
    {
        string? url = _config.GetValue<string>("HiveServerUrl");
        if (url == null)
        {
            return FailedLogin(ErrorCodes.FAILED_LOGIN);
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
                return FailedLogin(ErrorCodes.FAILED_VERIFY_LOGIN);
            }

            VerifyLoginRes? VerifyLoginRes = await resopnse.Content.ReadFromJsonAsync<VerifyLoginRes>();
            if (VerifyLoginRes == null)
            {
                return FailedLogin(ErrorCodes.FAILED_VERIFY_LOGIN_PARSING);
            }

            if (!VerifyLoginRes.IsSuccess)
            {
                return FailedLogin(ErrorCodes.FAILED_VERIFY_LOGIN);
            }

            bool IsSuccess = await _memoryRepo.SetAccessToken(id.ToString(), token);
            if (!IsSuccess)
            {
                return FailedLogin(ErrorCodes.FAILED_SET_TOKEN);
            }

            UserGameData? gameData = await GetOrCreateUserGameData(id);
            if (gameData == null)
            {
                await _memoryRepo.DeleteAccessToken(id.ToString());
                return FailedLogin(ErrorCodes.ERROR_USER_GAME_DATA);
            }

            return new LoginResult(ErrorCodes.NONE, gameData);

        }
        catch(Exception e)
        {
            System.Console.WriteLine($"Failed Hive Connect {e.Message}");
            return FailedLogin(ErrorCodes.FAILED_HIVE_CONNECT);
        }
    }

    private LoginResult FailedLogin(ErrorCodes errorCode)
    {
        return new LoginResult(errorCode, null);
    }

    // 함수는 하나의 일만 하도록 수정하기
    private async Task<UserGameData?> GetOrCreateUserGameData(Int64 id)
    {
        bool isExist = await _authRepo.CheckUserAsync(id);
        if (!isExist)
        {
            await CreateUserGameData(id);
        }
        return await _authRepo.GetUserGameDataAsync(id);
    }

    private async Task<bool> CreateUserGameData(Int64 id)
    {
        await _authRepo.CreateUserAsync(new UserGameData
        {
            user_id = id,
            user_name = "user_" + id,
            gold = 0,
            level = 1,
            exp = 0,
            win = 0,
            lose = 0
        });

        return true;
    }
}