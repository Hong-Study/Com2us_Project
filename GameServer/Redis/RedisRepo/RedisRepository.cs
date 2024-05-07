using CloudStructures;
using CloudStructures.Structures;
using Common;

namespace GameServer;

public class RedisRepository : IRedisRepository
{
    public RedisConnection _redisConn;
    public string _tokenKey = "_token";

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public RedisRepository(string connectionString)
    {
        var config = new RedisConfig("default", connectionString);
        _redisConn = new RedisConnection(config);
    }

    public async Task<ErrorCode> ValidateToken(Int64 userID, string token)
    {
        string key = userID.ToString() + _tokenKey;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpan.FromMinutes(30));
            var result = await redis.GetAsync();
            if(result.Value == token)
            {
                return ErrorCode.NONE;
            }

            return ErrorCode.INVALID_AUTH_TOKEN;
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            return ErrorCode.EXCEPTION_REDIS;
        }
    }
}