using Common;

public interface IRedisRepository
{
    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger);
    public Task<ErrorCode> ValidateToken(Int64 userID, string token);
}