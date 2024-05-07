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
    public async Task<VerifyLoginRes> VerifyLogin(VerifyLoginReq request)
    {
        _logger.LogInformation("VerifyLogin");

        AuthService.VerifyLoginResult result = await _service.VerifyLoginAsync(request);

        return new VerifyLoginRes()
        {
            ErrorCode = result.ErrorCode,
            IsSuccess = result.IsSuccess
        };
    }
}