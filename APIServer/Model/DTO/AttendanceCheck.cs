
using System.ComponentModel.DataAnnotations;

public class AttendanceCheckReq
{
    [Required(ErrorMessage = "Id is required")]
    public string UserID { get; set; } = null!;
    [Required(ErrorMessage = "Token is required")]
    public DateTime NowTime { get; set; }
}

public class AttendanceCheckRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}