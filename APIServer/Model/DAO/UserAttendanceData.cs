public class UserAttendanceData
{
    public string user_id { get; set; } = null!;
    public DateTime attendance_date { get; set; }
    public bool is_success { get; set; }
    public DateTime? created_at { get; set; }
}