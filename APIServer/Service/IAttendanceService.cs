public interface IAttendanceService
{
    public Task<AttendanceService.AttendanceResult> AttendanceCheck(string userId, DateTime nowTime);
}