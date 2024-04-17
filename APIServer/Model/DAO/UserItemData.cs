public class UserItemData
{
    public int item_id { get; set; }
    public int user_id { get; set; }
    public string item_name { get; set; } = null!;
    public string item_type { get; set; } = null!;
    public int item_price { get; set; }
    public int item_count { get; set; }
    public DateTime created_at { get; set; }
}