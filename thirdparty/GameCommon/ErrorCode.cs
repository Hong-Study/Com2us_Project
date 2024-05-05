namespace Common;

public enum ErrorCode : short
{
    NONE = 0,
    
    NOT_MATCH_PACKET_TYPE = 2000,
    DESERIALIZE_PACKET_ERROR = 2001,
    SERIALIZE_PACKET_ERROR = 2002,
    FULL_USER_COUNT = 2003,
    ALREADY_EXIST_USER = 2004,
    EXCEPTION_ADD_USER = 2005,
    NOT_EXIST_USER = 2006,
    NOT_EXIST_ROOM = 2007,
    FULL_ROOM_COUNT = 2008,
    EXCEPTION_ROOM_ENTER = 2009,
    NOT_EXIST_ROOM_USER = 2010,
    NOT_EXIST_ROOM_LEAVE_USER_DATA = 2011,
    NOT_EXIST_ROOM_CHAT_USER_DATA = 2012,
    NOT_EXIST_ROOM_READY_USER_DATA = 2013,
    NOT_EXIST_GAME_PUT_USER_DATA = 2014,
    NOT_START_GAME = 2015,
    ALREADY_START_GAME = 2016,
    NOT_MY_TURN = 2017,
    EXCEPTION_LOGIN_USER = 2018,
    ALREADY_IN_ROOM = 2019,
    NOT_IN_ROOM = 2020,
    CAN_NOT_LOGIN = 2021,
    FAILED_DATA_UPDATE = 2022,
    NOT_FOUND_USER_INFO = 2023,
    EXCEPTION_USER_DATABASE = 2024,
    INVALID_AUTH_TOKEN = 2025,
    EXCEPTION_REDIS = 2026,

}