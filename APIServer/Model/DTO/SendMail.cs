public class SendMailReq
{
    public string SendUserName { get; set; } = null!;
    public string RecvUserName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    // public int ItemId { get; set; }
    // public int ItemCount { get; set; }
}

public class SendMailRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}