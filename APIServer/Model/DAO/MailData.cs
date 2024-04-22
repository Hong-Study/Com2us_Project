public class MailData
{
    public int mail_id { get; set; }
    public string send_user_name { get; set; } = null!;
    public string recv_user_name { get; set; } = null!;
    public string mail_title { get; set; } = null!;
    public string mail_content { get; set; } = null!;
    public int item_id { get; set; }
    public int item_count { get; set; }
    public bool is_read { get; set; }
    public DateTime? created_at { get; set; }
}