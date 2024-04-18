public class ReadMailReq
{
    public int Id { get; set; }
}

public class ReadMailRes : DefaultRes
{
    public List<Mail> Mails { get; set; } = null!;
}

public class Mail
{
    public string RecvUserName { get; set; } = null!;
    public string SendUserName { get; set; } = null!;
    public string MailTitle { get; set; } = null!;
    public string MailContent { get; set; } = null!;
    public bool IsRead { get; set; }
}