
using System.ComponentModel.DataAnnotations;

public class AttendanceCheckReq
{
    [Required(ErrorMessage = "Id is required")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Token is required")]
    public DateTime NowTime { get; set; }
}

public class AttendanceCheckRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}