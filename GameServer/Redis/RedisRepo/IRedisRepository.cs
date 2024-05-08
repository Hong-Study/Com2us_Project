using Common;

public interface IRedisRepository
{
    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger);
    public Task<ErrorCode> ValidateToken(string userID, string token);
}