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
        // 서비스 입장에서는 HTTP로 주던지 어떤 형태로 주던지 상관 없기 때문에
        // 서비스와의 의존성이 강하기 때문에 분리하는 것을 노리기

        AuthService.LoginResult result = await _service.LoginAsync(request);
        return new LoginRes()
        {
            ErrorCode = result.ErrorCode,
            Id = result.Id,
            Token = result.Token
        };
    }
}