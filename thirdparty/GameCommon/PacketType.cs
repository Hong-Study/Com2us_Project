using System;

namespace Common;

public enum PacketType : Int16
{
    REQ_S_PING = 1000,
    RES_C_PONG = 1001,

    RES_S_CONNECT = 1002,

    REQ_C_LOGIN = 1003,
    RES_S_LOGIN = 1004,

    REQ_C_LOGOUT = 1005,
    RES_S_LOGOUT = 1006,

    REQ_C_ROOM_ENTER = 1007,
    RES_S_ROOM_ENTER = 1008,

    REQ_C_ROOM_LEAVE = 1009,
    RES_S_ROOM_LEAVE = 1010,

    REQ_S_USER_LEAVE = 1011,
    REQ_S_NEW_USER_ENTER = 1012,

    REQ_C_ROOM_CHAT = 1013,
    RES_S_ROOM_CHAT = 1014,

    REQ_C_GAME_READY = 1100,
    RES_S_GAME_READY = 1101,

    REQ_S_GAME_START = 1102,
    RES_C_GAME_START = 1103,

    RES_C_GAME_END = 1104,
    REQ_S_GAME_END = 1105,

    REQ_C_GAME_PUT = 1106,
    RES_S_GAME_PUT = 1107,

    REQ_S_TURN_CHANGE = 1108,
    RES_C_TURN_CHANGE = 1109,

    RES_C_GAME_CANCLE = 1110,
    REQ_S_GAME_CANCLE = 1111,

    N_GAME_TIMER = 1023,
}