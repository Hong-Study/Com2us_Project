using CloudStructures;
using CloudStructures.Structures;

public class MemoryRepository : IMemoryRepository
{
    public RedisConnection _redisConn;
    public string _tokenKey = "token_";
    public string _userStateKey = "_state";
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

    public async Task<bool> DeleteAccessToken(string userId)
    {
        string key = userId + _tokenKey;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.LoginTimeSpan());
            bool IsSuccess = await redis.DeleteAsync();


            return IsSuccess;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }

    // userId_token : token
    public async Task<string?> GetAccessToken(string userId)
    {
        string key = userId + _tokenKey;

        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.TicketKeyTimeSpan());
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
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.NxKeyTimeSpan());
            return await redis.SetAsync(token);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }

    public async Task<bool> SetUserState(string userID, string state)
    {
        string key = userID + _userStateKey;
        try
        {
            RedisString<string> redis = new(_redisConn, key, TimeSpanUtils.NxKeyTimeSpan());
            return await redis.SetAsync(state);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }
}