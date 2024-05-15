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
}