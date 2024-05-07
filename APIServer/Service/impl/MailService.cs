

public class MailService : IMailService
{
    IMailRepository _mailRepo;
    IAuthRepository _authRepo;
    public MailService(IMailRepository repo, IAuthRepository authRepo)
    {
        _mailRepo = repo;
        _authRepo = authRepo;
    }

    public record ReadMailAllResult(ErrorCode errorCode, IEnumerable<MailData>? mails);
    public async Task<ReadMailAllResult> ReadMailToAll(string userName)
    {
        var result = await _mailRepo.ReadMailToAll(userName);
        if (result == null)
        {
            return FailedReadMailAll(ErrorCode.FAILED_READ_MAIL);
        }

        return new ReadMailAllResult(ErrorCode.NONE, result);
    }

    public record SendMailResult(ErrorCode errorCode, bool isSuccess);
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
            return FailedSendMail(ErrorCode.FAILED_SEND_MAIL);
        }

        return new SendMailResult(ErrorCode.NONE, true);
    }

    public async Task<SendMailResult> SendMail(Int64 userId, string recvUserName, string title, string content, Int32 itemId = 0, Int32 itemCount = 0)
    {
        UserNameData? data = await _authRepo.GetUserNameDataAsync(userId);
        if(data == null)
        {
            return FailedSendMail(ErrorCode.NOT_FOUND_USER_NAME);
        }

        return await SendMail(data.user_name, recvUserName, title, content, itemId, itemCount);
    }

    public ReadMailAllResult FailedReadMailAll(ErrorCode errorCode)
    {
        return new ReadMailAllResult(errorCode, null);
    }

    public SendMailResult FailedSendMail(ErrorCode errorCode)
    {
        return new SendMailResult(errorCode, false);
    }
}