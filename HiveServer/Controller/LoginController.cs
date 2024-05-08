using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    ILogger<LoginController> _logger;
    IAuthService _service;
    public LoginController(IAuthService service, ILogger<LoginController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<LoginRes> Login([FromBody] LoginReq request)
    {
        _logger.LogInformation($"Login {request.Email}");

        AuthService.LoginResult result = await _service.LoginAsync(request);

        return new LoginRes()
        {
            ErrorCode = result.ErrorCode,
            Id = result.Id,
            Token = result.Token
        };
    }
}