namespace GameServer;

public class ServerPacketData
{
    public string SessionID { get; set; }
    public byte[] Body { get; set; }
    public Int16 PacketType { get; set; }

    public ServerPacketData(string SessionID, byte[] Body, Int16 PacketType)
    {
        this.SessionID = SessionID;
        this.Body = Body;
        this.PacketType = PacketType;
    }
}