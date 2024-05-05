using Common;

public interface IRedisRepository
{
    public Task<ErrorCode> ValidateToken(Int64 userID, string token);
}