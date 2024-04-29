using CloudStructures;

namespace GameServer;

public class MemoryManager : DataManager
{
    public RedisConnection _redisConn;

    public MemoryManager(string connectionString)
    {
        RedisConfig redisConfig = new("default", connectionString);
        _redisConn = new RedisConnection(redisConfig);
    }

   public override void InitHandler()
   {
        
   }
}