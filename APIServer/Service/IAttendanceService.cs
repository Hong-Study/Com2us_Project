public interface IAttendanceService
{
    public Task<AttendanceService.AttendanceResult> AttendanceCheck(int userId, DateTime nowTime);
}