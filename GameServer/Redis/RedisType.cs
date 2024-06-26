public enum RedisType
{
    NONE = 0,

    REDIS_PACKET_START = 5000,
    
    REQ_RD_USER_LOGIN = 5001,
    SET_RD_USER_STATE = 5002,

    REDIS_PACKET_END = 5999,
}

public class UserState
{
    public const string LOBBY = "LOBBY";
    public const string JOIN_MATCh = "MATCh";
    public const string COMPLETE_MATCH = "MATCCHING";
    public const string GAME = "GAME";
}