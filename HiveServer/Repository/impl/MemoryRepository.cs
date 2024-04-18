using CloudStructures;
using CloudStructures.Structures;

public class MemoryRepository : IMemoryRepository
{
    public RedisConnection _redisConn;
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

    public async Task<bool> DeleteAccessToken(int user_id)
    {
        // RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
        string key = user_id.ToString();
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

    public async Task<string?> GetAccessToken(int user_id)
    {
        string key = user_id.ToString();
        System.Console.WriteLine("GetAccessToken" + key);
        try
        {
            RedisString<string> redis = new(_redisConn, key, TicketKeyTimeSpan());
            RedisResult<string> token = await redis.GetAsync();
            if(!token.HasValue)
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

    public async Task<bool> SetAccessToken(int user_id, string token)
    {
        string key = user_id.ToString();
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

    // 따로 빼는것도 좋음
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