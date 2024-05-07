using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GetMailController : ControllerBase
{
    public IMailService _mailService;
    public ILogger<GetMailController> _logger;
    public GetMailController(IMailService mailService, ILogger<GetMailController> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<GetMailRes> Get([FromQuery] string userName)
    {
        _logger.LogInformation($"GetMailController Get: {userName}");
        
        MailService.ReadMailAllResult result = await _mailService.ReadMailToAll(userName);

        return new GetMailRes()
        {
            ErrorCode = result.errorCode,
            Mails = result.mails
        };
    }
}