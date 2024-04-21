public class GetMailReq
{
    public string UserName { get; set; } = null!;
}

public class GetMailRes : DefaultRes
{
    public List<Mail>? Mails { get; set; } = null!;
}

public class Mail
{
    public string SendUserName { get; set; } = null!;
    public string RecvUserName { get; set; } = null!;
    public string MailTitle { get; set; } = null!;
    public string MailContent { get; set; } = null!;
    public bool isRead { get; set; }
}