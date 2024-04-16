using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginCheckController : ControllerBase
{
    IAuthService _service;
    public LoginCheckController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<LoginCheckRes> CheckLogin([FromBody] LoginCheckReq request)
    {
        return await _service.LoginCheckAsync(request);
    }

    [HttpGet]
    public string Test()
    {
        return "LoginCheckController Test";
    }
}