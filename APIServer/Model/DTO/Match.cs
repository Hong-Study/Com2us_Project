using System.ComponentModel.DataAnnotations;

public class MatchingReq
{
    public string UserID { get; set; } = null!;
}

public class MatchingRes
{
    [Required]
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}

public class CancleMatchingReq
{
    public string UserID { get; set; } = null!;
}

public class CancleMatchingRes
{
    [Required]
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}


public class CheckMatchingReq
{
    public string UserID { get; set; } = null!;
}

public class CheckMatchingRes
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
    public string? ServerAddress { get; set; }
    public int Port { get; set; } = 0;
    public int RoomNumber { get; set; } = 0;
}