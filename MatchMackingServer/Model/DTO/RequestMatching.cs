
using Common;

public class MatchingRequest
{
    public string UserID { get; set; } = null!;
}

public class MatchResponse
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}