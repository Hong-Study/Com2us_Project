using System.Text;
using Newtonsoft.Json;

public class AuthService : IAuthService
{
    IAuthRepository _authRepo;
    IMemoryRepository _memoryRepo;
    IConfiguration _config;
    HttpClient _client = new HttpClient();
    public AuthService(IConfiguration config, IAuthRepository authRepository, IMemoryRepository memoryRepository)
    {
        _config = config;
        _authRepo = authRepository;
        _memoryRepo = memoryRepository;
    }

    public async Task<LoginRes> LoginAsync(LoginReq request)
    {
        string? url = _config.GetValue<string>("HiveServerUrl");
        if (url == null)
        {
            return FailedLogin(ErrorCodes.FAILED_LOGIN);
        }

        _client.BaseAddress = new Uri(url);
        try
        {
            LoginCheckReq loginCheckReq = new LoginCheckReq
            {
                Id = request.Id,
                Token = request.Token
            };

            HttpResponseMessage resopnse = await _client.PostAsJsonAsync("api/loginCheck", loginCheckReq);

            LoginCheckRes? loginCheckRes = await resopnse.Content.ReadFromJsonAsync<LoginCheckRes>();

            if (loginCheckRes == null)
            {
                return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
            }

            if (loginCheckRes.IsSuccess)
            {
                bool IsSuccess = await _memoryRepo.SetAccessToken(request.Id, request.Token);
                if (!IsSuccess)
                {
                    return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
                }

                UserGameData? gameData = await GetUserGameData(request.Id);
                if (gameData == null)
                {
                    return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
                }

                return new LoginRes
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    GameData = gameData
                };
            }
            else
            {
                return FailedLogin(ErrorCodes.INTERNAL_SERVER_ERROR);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthService " + e.Message);
            return FailedLogin(ErrorCodes.FAILED_LOGIN);
        }
    }

    private LoginRes FailedLogin(ErrorCodes errorCode)
    {
        return new LoginRes
        {
            StatusCode = 400,
            ErrorCode = errorCode,
            IsSuccess = false
        };
    }

    private async Task<UserGameData?> GetUserGameData(int id)
    {
        System.Console.WriteLine("GetUserGameData " + id);
        bool isExist = await _authRepo.CheckUserAsync(id);
        if (!isExist)
        {
            await _authRepo.CreateUserAsync(new UserGameData
            {
                id = id,
                gold = 0,
                level = 1,
                exp = 0,
                win = 0,
                lose = 0
            });
        }
        return await _authRepo.GetUserGameDataAsync(id);
    }
}