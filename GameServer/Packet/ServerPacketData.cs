namespace GameServer;

public class ServerPacketData
{
    public ClientSession Session {get; set;}
    public byte[] Body {get; set;}
    public Int16 PacketType {get; set;}

    public ServerPacketData(ClientSession session, byte[] body, Int16 type)
    {
        Session = session;
        Body = body;
        PacketType = type;
    }
}