using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    IAuthService _service;
    public LoginController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<LoginRes> Login([FromBody] LoginReq request)
    {
        return await _service.LoginAsync(request);
    }

      [HttpGet]
    public string Test()
    {
        return "LoginController Test";
    }
}