using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GetMailController : ControllerBase
{
    public IMailService _mailService;
    public GetMailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}