public class AttendanceService : IAttendanceService
{
    IAttendanceCheckRepository _attendanceRepo;
    IMailService _mailService;
    public AttendanceService(IAttendanceCheckRepository repo, IMailService mailService)
    {
        _attendanceRepo = repo;
        _mailService = mailService;
    }

    public record AttendanceResult(ErrorCode errorCode, bool isSuccess);
    public async Task<AttendanceResult> AttendanceCheck(string userId, DateTime nowTime)
    {
        // 서버와의 시간을 체크
        try
        {
            DateTime clientDate = GetYearMonthDay(nowTime);
            DateTime serverDate = GetYearMonthDay(DateTime.Now);

            if (clientDate != serverDate)
            {
                return FailedAttendance(ErrorCode.NOT_SAME_TIME);
            }

            // 출석 여부를 체크한다.
            bool isAttendance = await _attendanceRepo.IsAttendanceCheck(userId, serverDate);
            if (isAttendance == true)
            {
                return FailedAttendance(ErrorCode.ALREADY_ATTENDANCE);
            }

            bool isSuccess = await _attendanceRepo.SetAttendanceCheck(new UserAttendanceData
            {
                user_id = userId,
                attendance_date = serverDate,
                is_success = true,
            });

            if (isSuccess == false)
            {
                return FailedAttendance(ErrorCode.FAILED_ATTENDANCE);
            }

            MailService.SendMailResult result = await _mailService.SendMail(userId, "GM", "출석 완료", "출석이 완료되었습니다.");
            if (result.isSuccess == false)
            {
                return FailedAttendance(ErrorCode.FAILED_SEND_MAIL);
            }

            return new AttendanceResult(ErrorCode.NONE, true);
        }
        catch
        {
            return FailedAttendance(ErrorCode.FAILED_ATTENDANCE);
        }
    }

    public DateTime GetYearMonthDay(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day);
    }

    public AttendanceResult FailedAttendance(ErrorCode errorCode)
    {
        return new AttendanceResult(errorCode, false);
    }
}