using CloudStructures;

namespace GameServer;

public class RedisConnector
{
    public RedisConnection RedisCon;

    public RedisConnector(string connectionString)
    {
        var config = new RedisConfig("default", connectionString);
        RedisCon = new RedisConnection(config);
    }
}