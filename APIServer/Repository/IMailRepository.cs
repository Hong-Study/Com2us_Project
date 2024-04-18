public interface IMailRepository
{
    public void SendMail(string to, string title, string content);
    public void ReadMailToAll(int userId);
}