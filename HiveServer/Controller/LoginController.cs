using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    IAccountService _service;
    public LoginController(IAccountService service)
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