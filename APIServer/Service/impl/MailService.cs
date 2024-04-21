

public class MailService : IMailService
{
    IMailRepository _mailRepo;
    public MailService(IMailRepository repo)
    {
        _mailRepo = repo;
    }

    public record ReadMailAllResult(ErrorCodes errorCode, List<Mail>? mails);
    public async Task<ReadMailAllResult> ReadMailToAll(string userName)
    {
        var result = await _mailRepo.ReadMailToAll(userName);
        if (result == null)
        {
            return FailedReadMailAll(ErrorCodes.FAILED_READ_MAIL);
        }

        List<Mail> mails = new List<Mail>();
        foreach (MailData mail in result)
        {
            mails.Add(new Mail()
            {
                SendUserName = mail.send_user_name,
                RecvUserName = mail.recv_user_name,
                MailTitle = mail.mail_title,
                MailContent = mail.mail_content,
                isRead = mail.is_read
            });
        }

        return new ReadMailAllResult(ErrorCodes.NONE, mails);
    }

    public record SendMailResult(ErrorCodes errorCode, bool isSuccess);
    public async Task<SendMailResult> SendMail(string sendUserName, string recvUserName, string title, string content)
    {
        MailData mailData = new MailData()
        {
            send_user_name = sendUserName,
            recv_user_name = recvUserName,
            mail_title = title,
            mail_content = content,
            is_read = false,
        };

        bool result = await _mailRepo.SendMail(mailData);
        if (!result)
        {
            return FailedSendMail(ErrorCodes.FAILED_SEND_MAIL);
        }

        return new SendMailResult(ErrorCodes.NONE, true);
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