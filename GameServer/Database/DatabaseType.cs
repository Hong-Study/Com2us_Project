public enum DatabaseType
{
    NONE = 0,

    DB_PACKET_START = 4000,
    
    REQ_DB_USER_LOGIN = 4001,
    REQ_DB_UPDATE_WIN_LOSE_COUNT = 4002,

    DB_PACKET_END = 4999,
}