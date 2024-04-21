public class UserGameData
{
    public Int64 user_id { get; set; }
    public string user_name { get; set; } = null!;
    public int level { get; set; }
    public int exp { get; set; }
    public int gold { get; set; }
    public int win { get; set; }
    public int lose { get; set; }
    public DateTime created_at { get; set; }
}

public class UserIdData
{
    public Int64 user_id { get; set; }
}