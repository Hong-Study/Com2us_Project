using CloudStructures;
using CloudStructures.Structures;
using Common;

namespace GameServer;

// Interface를 사용하는 이유를 다시 한번 생각해보아라
// 굳이 필요가 없다.
// 애자일론에서 굳이 미래를 예측하지 마라
// 완전하게 할 수 있는 것에 집중해라

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

    public async Task<ErrorCode> ValidateToken(string userID, string token)
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