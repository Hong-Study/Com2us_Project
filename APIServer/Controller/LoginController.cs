using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    IAuthService _authService;
    ILogger<LoginController> _logger;
    public LoginController(IAuthService authService, ILogger<LoginController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<LoginRes> Post([FromBody] LoginReq request)
    {
        AuthService.LoginResult result = await _authService.LoginAsync(request.UserId, request.Token);    
        if(result.errorCode != ErrorCode.NONE)
        {
            _logger.LogError($"Login failed: {result.errorCode.ToString()}");
        }

        _logger.LogInformation($"Login success: {result.gameData?.user_name}");

        return new LoginRes
        {
            ErrorCode = result.errorCode,
            GameData = result.gameData,
            GameServerAddress = result.gameServerAddress,
            GameServerPort = result.gameServerPort
        };
    }
}