namespace Common;

public enum ErrorCode : Int16
{
    NONE = 0,
    INVALID_REQUEST = 100,
    INVALID_EMAIL = 101,
    INVALID_PASSWORD = 102,
    EMAIL_ALREADY_EXISTS = 103,
    EMAIL_DOES_NOT_EXIST = 104,
    PASSWORD_DOES_NOT_MATCH = 105,
    NOT_LOGIN = 106,
    INVALID_TOKEN = 107,
    TOKEN_EXPIRED = 108,

    FAILED_LOGIN = 200,
    FAILED_HIVE_CONNECT = 201,
    FAILED_VERIFY_LOGIN = 202,
    FAILED_VERIFY_LOGIN_PARSING = 203,
    FAILED_SET_TOKEN = 204,
    ERROR_USER_GAME_DATA = 205,
    NOT_SAME_TIME = 206,
    ALREADY_ATTENDANCE = 207,
    FAILED_ATTENDANCE = 208,
    FAILED_READ_MAIL = 209,
    FAILED_SEND_MAIL = 210,
    NOT_FOUND_USER_NAME = 211,

    INTERNAL_SERVER_ERROR = 500,


    MATCHING_FAIL = 1000,
    MATCHING_SERVER_ERROR = 1001,
    MATCHING_ALEARY_MATCHED = 1002,
    MATCHING_NOT_FOUND_USER = 1003,
    MATCHING_ALREADY_COMPLETE = 1004,
    MATCHING_JOIN_FAIL = 1005,

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
    NOT_LOGIN_USER = 2027,
    NOT_MATCHING_ROOM = 2028,
    NOT_MATCHING_USER = 2029,
}