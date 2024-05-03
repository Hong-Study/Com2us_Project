using MemoryPack;

public class UserGameData
{
    public Int64 user_id { get; set; }
    public string user_name { get; set; } = null!;
    public Int32 level { get; set; }
    public Int32 exp { get; set; }
    public Int32 gold { get; set; }
    public Int32 win { get; set; }
    public Int32 lose { get; set; }
    public DateTime created_at { get; set; }
}