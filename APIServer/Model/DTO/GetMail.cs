public class GetMailReq
{
    public string UserName { get; set; } = null!;
}

public class GetMailRes : DefaultRes
{
    public IEnumerable<MailData>? Mails { get; set; } = null!;
}