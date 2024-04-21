public interface IMailRepository
{
    public Task<IEnumerable<MailData>?> ReadMailToAll(string userName);
    public Task<bool> SendMail(MailData mailData);
}