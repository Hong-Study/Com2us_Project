public interface IMailService
{
    public Task<MailService.SendMailResult> SendMail(string sendUserName, string recvUserName, string title, string content);
    public Task<MailService.ReadMailAllResult> ReadMailToAll(string userName);
}