public class GetMailReq
{
    public int Id { get; set; }
}

public class GetMailRes : DefaultRes
{
    public List<Mail> Mails { get; set; } = null!;
}

public class Mail
{
    public int Id { get; set; }
    public string MailTitle { get; set; } = null!;
    public string MailContent { get; set; } = null!;
    public bool IsRead { get; set; }
}