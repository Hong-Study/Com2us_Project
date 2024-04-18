public class AttendanceService : IAttendanceService
{
    IAttendanceCheckRepository _attendanceRepo;
    ILogger<AttendanceService> _logger;

    public AttendanceService(IAttendanceCheckRepository repo, ILogger<AttendanceService> logger)
    {
        _attendanceRepo = repo;
        _logger = logger;
    }

    public record AttendanceResult(ErrorCodes errorCode, bool isSuccess);
    public async Task<AttendanceResult> AttendanceCheck(AttendanceCheckReq request)
    {
        // 서버와의 시간을 체크
        try
        {
            DateTime clientDate = GetYearMonthDay(request.NowTime);
            DateTime serverDate = GetYearMonthDay(DateTime.Now);

            if (clientDate != serverDate)
            {
                return FailedAttendance(ErrorCodes.NOT_SAME_TIME);
            }

            // 출석 여부를 체크한다.
            bool isAttendance = await _attendanceRepo.IsAttendanceCheck(request.UserId, clientDate);
            if (isAttendance == true)
            {
                return FailedAttendance(ErrorCodes.ALREADY_ATTENDANCE);
            }

            bool isSuccess = await _attendanceRepo.SetAttendanceCheck(new UserAttendanceData
            {
                user_id = request.UserId,
                attendance_date = clientDate,
                is_success = true,
            });

            if (isSuccess == false)
            {
                return FailedAttendance(ErrorCodes.FAILED_ATTENDANCE);
            }

            // 이후에는 해당 유저의 출석 여부를 반환한다.
            return new AttendanceResult(ErrorCodes.NONE, true);
        }
        catch
        {
            return FailedAttendance(ErrorCodes.FAILED_ATTENDANCE);
        }
    }

    public DateTime GetYearMonthDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day);
    }

    public AttendanceResult FailedAttendance(ErrorCodes errorCode)
    {
        return new AttendanceResult(errorCode, false);
    }
}