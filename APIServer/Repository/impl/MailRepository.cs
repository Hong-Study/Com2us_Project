using SqlKata.Execution;

public class MailRepository : DefaultDbConnection, IMailRepository
{
    ILogger<MailRepository> _logger;
    public MailRepository(ILogger<MailRepository> logger, IConfiguration config) : base(config)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<MailData>?> ReadMailToAll(string userName)
    {
        try
        {
            IEnumerable<MailData>? result = await _queryFactory.Query("mail_data")
                .Where("recv_user_name", userName)
                .GetAsync<MailData>();

            foreach(MailData mail in result)
            {
                mail.is_read = true;
                await _queryFactory.Query("mail_data")
                    .Where("mail_id", mail.mail_id)
                    .UpdateAsync(mail);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("MailRepository[ReadMailToAll] " + e.Message);
            return null;
        }
    }

    public async Task<bool> SendMail(MailData mailData)
    {
        try
        {
            Int32 count = await _queryFactory.Query("mail_data").InsertAsync(mailData);
            if(count == 0)
            {
                return false;
            }
            
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("MailRepository[SendMail] " + e.Message);
            return false;
        }
    }
}