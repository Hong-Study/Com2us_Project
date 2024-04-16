public class UserDB
{
    public int id { get; set; }
    public string password { get; set; } = null!;
    public string email { get; set; } = null!;
    public DateTime? created_at { get; set; }
}