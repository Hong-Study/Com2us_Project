using CloudStructures;
using CloudStructures.Structures;

public class MemoryRepository : IMemoryRepository
{
    public RedisConnection _redisConn;
    public string _tokenKey = "_token";
    public MemoryRepository(IConfiguration config)
    {
        string? connectionString = config.GetConnectionString("Redis");
        if (connectionString == null)
        {
            throw new Exception("Redis ConnectionString is null");
        }

        RedisConfig redisConfig = new("default", connectionString);
        _redisConn = new RedisConnection(redisConfig);
    }

    public async Task<bool> DeleteAccessToken(string id)
    {
        // RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
        string key = id + _tokenKey;
        try
        {
            RedisString<string> redis = new(_redisConn, key, LoginTimeSpan());
            bool IsSuccess = await redis.DeleteAsync();
            return IsSuccess;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }

    // id : token
    // id_token : token
    public async Task<string?> GetAccessToken(string id)
    {
        string key = id + _tokenKey;

        try
        {
            RedisString<string> redis = new(_redisConn, key, TicketKeyTimeSpan());
            RedisResult<string> token = await redis.GetAsync();
            if (!token.HasValue)
            {
                System.Console.WriteLine("Token is null");
                return null;
            }

            return token.Value;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<bool> SetAccessToken(string userId, string token)
    {
        string key = userId + _tokenKey;
        try
        {
            RedisString<string> redis = new(_redisConn, key, NxKeyTimeSpan());
            return await redis.SetAsync(token);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }

    // TimeSpan을 관리해주는 따른 파일로 만들어주는 것이 좋다
    public TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
    }

    public TimeSpan TicketKeyTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
        // return TimeSpan.FromSeconds(RediskeyExpireTime.TicketKeyExpireSecond);
    }

    public TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
        // return TimeSpan.FromSeconds(RediskeyExpireTime.NxKeyExpireSecond);
    }
}