using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    IAuthService _authService;
    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<LoginRes> Login([FromBody] LoginReq request)
    {
        return await _authService.LoginAsync(request);
    }

    [HttpGet]
    public string Test()
    {
        return "Hello World";
    }
}