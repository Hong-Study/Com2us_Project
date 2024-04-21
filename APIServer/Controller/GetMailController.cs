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
        MailService.ReadMailAllResult result = await _mailService.ReadMailToAll(userName);
        if (result.errorCode != ErrorCodes.NONE)
        {
            _logger.LogError($"Read mail failed: {result.errorCode.ToString()}");
        }

        return new GetMailRes()
        {
            ErrorCode = result.errorCode,
            Mails = result.mails
        };
    }
}