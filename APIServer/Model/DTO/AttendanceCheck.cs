
using System.ComponentModel.DataAnnotations;

public class AttendanceCheckReq
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Token is required")]
    public DateTime NowTime { get; set; }
}

public class AttendanceCheckRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}