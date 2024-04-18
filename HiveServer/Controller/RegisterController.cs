using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    IAuthService _service;
    ILogger<RegisterController> _logger;
    public RegisterController(IAuthService service, ILogger<RegisterController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<RegisterRes> Register([FromBody] RegisterReq request)
    {
        AuthService.RegisterResut result = await _service.RegisterAsync(request);
        if (result.ErrorCode != ErrorCodes.NONE)
        {
            _logger.LogError($"Register failed: {result.ErrorCode}");
        }

        return new RegisterRes()
        {
            ErrorCode = result.ErrorCode,
            IsSuccess = result.IsSuccess
        };
    }
}