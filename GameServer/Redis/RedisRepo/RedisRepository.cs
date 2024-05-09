using CloudStructures;
using CloudStructures.Structures;
using Common;

namespace GameServer;

public class RedisRepository
{
    public string _tokenKey = "_token";

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public RedisRepository() { }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public ErrorCode ValidateToken(string userID, string token, RedisConnector connector)
    {
        string key = userID.ToString() + _tokenKey;
        try
        {
            RedisString<string> redis = new(connector.RedisCon, key, TimeSpan.FromMinutes(30));
            var result = redis.GetAsync().Result;
            if (result.Value == token)
            {
                return ErrorCode.NONE;
            }

            return ErrorCode.INVALID_AUTH_TOKEN;
        }
        catch (Exception ex)
        {
            Logger.Error("Redis " + ex.Message);
            return ErrorCode.EXCEPTION_REDIS;
        }
    }
}