public abstract class DefaultRes
{
    public int StatusCode { get; set; } = 200;
    public ErrorCodes ErrorCode { get; set; } = ErrorCodes.NONE;
}