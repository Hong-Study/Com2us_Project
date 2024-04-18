public class MailRepository : DefaultDbConnection, IMailRepository
{
    public MailRepository(IConfiguration config) : base(config)
    {
        
    }

    public void ReadMailToAll(int userId)
    {
        throw new NotImplementedException();
    }

    public void SendMail(string to, string title, string content)
    {
        throw new NotImplementedException();
    }
}