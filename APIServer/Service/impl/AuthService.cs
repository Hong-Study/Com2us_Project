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
    public async Task<LoginResult> LoginAsync(LoginReq request)
    {
        string? url = _config.GetValue<string>("HiveServerUrl");
        if (url == null)
        {
            return FailedLogin(ErrorCodes.FAILED_LOGIN);
        }

        _client.BaseAddress = new Uri(url);

        try
        {
            VerifyLoginReq VerifyLoginReq = new VerifyLoginReq
            {
                UserId = request.UserId,
                Token = request.Token
            };

            HttpResponseMessage resopnse = await _client.PostAsJsonAsync("api/verifylogin", VerifyLoginReq);
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

            bool IsSuccess = await _memoryRepo.SetAccessToken(request.UserId.ToString(), request.Token);
            if (!IsSuccess)
            {
                return FailedLogin(ErrorCodes.FAILED_SET_TOKEN);
            }

            UserGameData? gameData = await GetOrCreateUserGameData(request.UserId);
            if (gameData == null)
            {
                return FailedLogin(ErrorCodes.ERROR_USER_GAME_DATA);
            }

            return new LoginResult(ErrorCodes.NONE, gameData);

        }
        catch
        {
            return FailedLogin(ErrorCodes.FAILED_HIVE_CONNECT);
        }
    }

    private LoginResult FailedLogin(ErrorCodes errorCode)
    {
        return new LoginResult(errorCode, null);
    }

    // 함수는 하나의 일만 하도록 수정하기
    private async Task<UserGameData?> GetOrCreateUserGameData(int id)
    {
        System.Console.WriteLine("GetUserGameData " + id);
        bool isExist = await _authRepo.CheckUserAsync(id);
        if (!isExist)
        {
            await CreateUserGameData(id);
        }
        return await _authRepo.GetUserGameDataAsync(id);
    }

    private async Task<bool> CreateUserGameData(int id)
    {
        await _authRepo.CreateUserAsync(new UserGameData
        {
            user_id = id,
            user_name = "user" + id,
            gold = 0,
            level = 1,
            exp = 0,
            win = 0,
            lose = 0
        });

        return true;
    }
}