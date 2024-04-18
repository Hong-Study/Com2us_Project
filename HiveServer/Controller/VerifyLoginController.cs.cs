using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VerifyLoginController : ControllerBase
{
    IAuthService _service;
    ILogger<VerifyLoginController> _logger;
    public VerifyLoginController(IAuthService service, ILogger<VerifyLoginController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<VerifyLoginRes> CheckLogin(VerifyLoginReq request)
    {
        AuthService.VerifyLoginResult result = await _service.VerifyLoginAsync(request);
        if (result.ErrorCode != ErrorCodes.NONE)
        {
            _logger.LogError($"Register failed: {result.ErrorCode}");
        }

        return new VerifyLoginRes()
        {
            ErrorCode = result.ErrorCode,
            IsSuccess = result.IsSuccess
        };
    }
}