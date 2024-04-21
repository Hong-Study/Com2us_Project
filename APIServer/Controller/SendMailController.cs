using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SendMailController : ControllerBase
{
    public IMailService _mailService;
    public ILogger<SendMailController> _logger;
    public SendMailController(IMailService mailService, ILogger<SendMailController> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<SendMailRes> Post([FromHeader(Name ="UserId")] int userId
                                        , [FromBody] SendMailReq request)
    {
        MailService.SendMailResult result = await _mailService.SendMail(
                                request.SendUserName, request.RecvUserName
                                , request.Title, request.Content);

        if(result.errorCode != ErrorCodes.NONE)
        {
            _logger.LogError($"Send mail failed: {result.errorCode.ToString()}");
        }

        return new SendMailRes()
        {
            ErrorCode = result.errorCode,
            IsSuccess = result.isSuccess
        };
    }
}