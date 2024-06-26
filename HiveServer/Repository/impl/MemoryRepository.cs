using CloudStructures;
using CloudStructures.Structures;

public class MemoryRepository : IMemoryRepository
{
    public ILogger<MemoryRepository> _logger;
    public RedisConnection _redisConn;
    public MemoryRepository(IConfiguration config, ILogger<MemoryRepository> logger)
    {
        _logger = logger;

        string? connectionString = config.GetConnectionString("Redis");
        if (connectionString == null)
        {
            throw new Exception("Redis ConnectionString is null");
        }

        RedisConfig redisConfig = new("default", connectionString);
        _redisConn = new RedisConnection(redisConfig);
    }

    public async Task<bool> DeleteAccessToken(string userID)
    {
        string key = userID;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.LoginTimeSpan());
            bool IsSuccess = await redis.DeleteAsync();
            return IsSuccess;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return false;
        }
    }

    public async Task<string?> GetAccessToken(string userID)
    {
        string key = userID;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.TicketKeyTimeSpan());
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
            _logger.LogError(e.Message);

            return null;
        }

    }

    public async Task<bool> SetAccessToken(string userID, string token)
    {
        string key = userID;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.NxKeyTimeSpan());
            return await redis.SetAsync(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return false;
        }
    }

    // 따로 빼는것도 좋음
    
}