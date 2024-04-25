namespace GameServer;

public enum PacketType : Int16
{
    LOGIN = 0,
    LOGOUT = 1,
    ROOM_ENTER = 2,
    ROOM_LEAVE = 3,
    ROOM_CHAT = 4,
}