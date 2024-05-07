public interface IAttendanceService
{
    public Task<AttendanceService.AttendanceResult> AttendanceCheck(Int64 userId, DateTime nowTime);
}