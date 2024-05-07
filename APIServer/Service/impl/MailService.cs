

public class MailService : IMailService
{
    IMailRepository _mailRepo;
    IAuthRepository _authRepo;
    public MailService(IMailRepository repo, IAuthRepository authRepo)
    {
        _mailRepo = repo;
        _authRepo = authRepo;
    }

    public record ReadMailAllResult(ErrorCodes errorCode, IEnumerable<MailData>? mails);
    public async Task<ReadMailAllResult> ReadMailToAll(string userName)
    {
        var result = await _mailRepo.ReadMailToAll(userName);
        if (result == null)
        {
            return FailedReadMailAll(ErrorCodes.FAILED_READ_MAIL);
        }

        return new ReadMailAllResult(ErrorCodes.NONE, result);
    }

    public record SendMailResult(ErrorCodes errorCode, bool isSuccess);
    public async Task<SendMailResult> SendMail(string sendUserName, string recvUserName, string title, string content, Int32 itemId = 0, Int32 itemCount = 0)
    {
        MailData mailData = new MailData()
        {
            send_user_name = sendUserName,
            recv_user_name = recvUserName,
            mail_title = title,
            mail_content = content,
            item_id = itemId,
            item_count = itemCount,
            is_read = false,
        };

        bool result = await _mailRepo.SendMail(mailData);
        if (!result)
        {
            return FailedSendMail(ErrorCodes.FAILED_SEND_MAIL);
        }

        return new SendMailResult(ErrorCodes.NONE, true);
    }

    public async Task<SendMailResult> SendMail(Int64 userId, string recvUserName, string title, string content, Int32 itemId = 0, Int32 itemCount = 0)
    {
        UserNameData? data = await _authRepo.GetUserNameDataAsync(userId);
        if(data == null)
        {
            return FailedSendMail(ErrorCodes.NOT_FOUND_USER_NAME);
        }

        return await SendMail(data.user_name, recvUserName, title, content, itemId, itemCount);
    }

    public ReadMailAllResult FailedReadMailAll(ErrorCodes errorCode)
    {
        return new ReadMailAllResult(errorCode, null);
    }

    public SendMailResult FailedSendMail(ErrorCodes errorCode)
    {
        return new SendMailResult(errorCode, false);
    }
}