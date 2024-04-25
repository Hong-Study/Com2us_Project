namespace GameServer;

public enum ErrorCode : short
{
    NONE = 0,

    NOT_MATCH_PACKET_TYPE = 99,
    DESERIALIZE_PACKET_ERROR = 100,
    SERIALIZE_PACKET_ERROR = 101,
}