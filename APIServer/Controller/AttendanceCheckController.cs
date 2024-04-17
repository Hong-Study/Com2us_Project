using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AttendanceCheckController : ControllerBase
{
    IAttendanceCheckService _attendanceCheckService;
    public AttendanceCheckController(IAttendanceCheckService attendanceCheckService)
    {
        _attendanceCheckService = attendanceCheckService;
    }

    [HttpPost]
    public AttendanceCheckRes Post(AttendanceCheckReq attendanceCheck)
    {
        return new AttendanceCheckRes
        {
            IsSuccess = true
        };
    }
}