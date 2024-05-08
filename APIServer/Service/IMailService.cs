public interface IMailService
{
    public Task<MailService.SendMailResult> SendMail(string sendUserName, string recvUserName, string title, string content, Int32 itemId = 0, Int32 itemCount = 0);
    // public Task<MailService.SendMailResult> SendMail(string userId, string recvUserName, string title, string content, Int32 itemId = 0, Int32 itemCount = 0);
    public Task<MailService.ReadMailAllResult> ReadMailToAll(string userName);
}