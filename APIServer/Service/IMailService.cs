public interface IMailService
{
    public Task<MailService.SendMailResult> SendMail(string sendUserName, string recvUserName, string title, string content, int itemId = 0, int itemCount = 0);
    public Task<MailService.SendMailResult> SendMail(long userId, string recvUserName, string title, string content, int itemId = 0, int itemCount = 0);
    public Task<MailService.ReadMailAllResult> ReadMailToAll(string userName);
}