public class UserAttendanceData
{
    public long user_id { get; set; }
    public DateTime attendance_date { get; set; }
    public bool is_success { get; set; }
    public DateTime? created_at { get; set; }
}