public interface IAttendanceService
{
    public Task<AttendanceService.AttendanceResult> AttendanceCheck(AttendanceCheckReq req);
}