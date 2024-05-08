public class UserGameData
{
    public string user_id { get; set; } = null!;
    public string user_name { get; set; } = null!;
    public Int32 level { get; set; }
    public Int32 exp { get; set; }
    public Int32 gold { get; set; }
    public Int32 win { get; set; }
    public Int32 lose { get; set; }
    public DateTime? created_at { get; set; }
}

public class UserIdData
{
    public string user_id { get; set; } = null!;
}

public class UserNameData
{
    public string user_name { get; set; } = null!;
}