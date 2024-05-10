public class LogManager
{
    public enum EventType
    {
        CreateAccount = 101,
        Login = 201,
        LoginAddRedis = 202, 
        CreateCharacter = 205,
    }

    private static ILoggerFactory s_loggerFactory = null!;

    public static Dictionary<EventType, EventId> EventIdDic = new()
    {
        { EventType.CreateAccount, new EventId((Int32)EventType.CreateAccount, "CreateAccount") },
        { EventType.Login, new EventId((Int32)EventType.Login, "Login") },
        { EventType.LoginAddRedis, new EventId((Int32)EventType.LoginAddRedis, "LoginAddRedis") },
        { EventType.CreateCharacter, new EventId((Int32)EventType.CreateCharacter, "CreateCharacter") },
    };

    public static ILogger Logger { get; private set; } = null!;

    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        s_loggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>() where T : class
    {
        return s_loggerFactory.CreateLogger<T>();
    }
}