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
    public async Task<AttendanceCheckRes> Post([FromHeader(Name ="UserId")] Int64 userId,
                                                [FromBody] AttendanceCheckReq request)
    {
        _logger.LogInformation($"AttendanceCheckController Post: {userId} -> {request.UserId}");

        AttendanceService.AttendanceResult result = await _attendanceCheckService.AttendanceCheck(userId, request.NowTime);

        return new AttendanceCheckRes
        {
            ErrorCode = result.errorCode,
            IsSuccess = result.isSuccess
        };
    }
}