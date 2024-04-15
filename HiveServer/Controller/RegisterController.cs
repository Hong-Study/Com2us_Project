using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    IAccountService _service;
    public RegisterController(IAccountService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<RegisterRes> Register([FromBody] RegisterReq request)
    {
        System.Console.WriteLine(request.Email + " " + request.Password);
        return await _service.CreateAccountAsync(request);
    }

    [HttpGet]
    public string Test()
    {
        return "RegisterController Test";
    }
}