public class MailData
{
    public int id { get; set; }
    public int user_id { get; set; }
    public string mail_title { get; set; } = null!;
    public string mail_content { get; set; } = null!;
    public bool is_read { get; set; }
}