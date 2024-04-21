using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AttendanceCheckController : ControllerBase
{
    IAttendanceService _attendanceCheckService;
    ILogger<AttendanceCheckController> _logger;
    public AttendanceCheckController(IAttendanceService attendanceCheckService, ILogger<AttendanceCheckController> logger)
    {
        _attendanceCheckService = attendanceCheckService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<AttendanceCheckRes> Post([FromHeader(Name ="UserId")] int userId,
                                                [FromBody] AttendanceCheckReq request)
    {
        AttendanceService.AttendanceResult result = await _attendanceCheckService.AttendanceCheck(userId, request.NowTime);
        if (result.errorCode != ErrorCodes.NONE)
        {
            _logger.LogError($"Attendance check failed: {result.errorCode.ToString()}");
        }

        return new AttendanceCheckRes
        {
            ErrorCode = result.errorCode,
            IsSuccess = result.isSuccess
        };
    }
}