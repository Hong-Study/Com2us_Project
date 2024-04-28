using CloudStructures;

public class MemoryManager
{
    public RedisConnection _redisConn;
    public MemoryManager(string connectionString)
    {
        RedisConfig redisConfig = new("default", connectionString);
        _redisConn = new RedisConnection(redisConfig);
    }
}