using CloudStructures;
using CloudStructures.Structures;
using StackExchange.Redis;

public class RedisLockRepository
{
    public RedisConnection _redisConn;
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

        _lockTime = TimeSpan.FromMinutes(1);
    }

    public async Task<bool> LockAsync(string UserID)
    {
        RedisKey key = UserID + _tokenKey;
        var redisLock = new RedisLock<Int32>(_redisConn, key);

        return await redisLock.TakeAsync(1, _lockTime);
    }

    public async Task<bool> UnLockAsync(string UserID)
    {
        RedisKey key = UserID + _tokenKey;
        var redisLock = new RedisLock<Int32>(_redisConn, key);

        return await redisLock.ReleaseAsync(1);
    }
}