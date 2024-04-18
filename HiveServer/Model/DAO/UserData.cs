public class UserData
{
    public int user_id { get; set; }
    public string password { get; set; } = null!;
    public string email { get; set; } = null!;
    public DateTime? created_at { get; set; }
}