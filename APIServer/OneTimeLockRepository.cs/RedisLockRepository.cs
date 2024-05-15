using CloudStructures;
using CloudStructures.Structures;
using StackExchange.Redis;

public class RedisLockRepository
{
    RedisConnection _redisConn;
    TimeSpan _lockTime;
    public string _tokenKey = "_lock";
    public RedisLockRepository(IConfiguration config)
    {
        string? connectionString = config.GetConnectionString("Redis");
        if (connectionString == null)
        {
            throw new Exception("Redis ConnectionString is null");
        }

        RedisConfig redisConfig = new("default", connectionString);
        _redisConn = new RedisConnection(redisConfig);

        _lockTime = TimeSpan.FromSeconds(10);
    }

    public async Task<bool> LockAsync(string UserID)
    {
        string key = UserID + _tokenKey;
        var redisLock = new RedisLock<string>(_redisConn, key);

        return await redisLock.TakeAsync("LockValue", _lockTime);
    }

    public async Task<bool> UnLockAsync(string UserID)
    {
        string key = UserID + _tokenKey;
        var redisLock = new RedisLock<string>(_redisConn, key);

        return await redisLock.ReleaseAsync("LockValue");
    }
}