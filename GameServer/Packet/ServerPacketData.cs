namespace GameServer;

public class ServerPacketData
{
    public string sessionID { get; set; }
    public byte[] Body { get; set; }
    public Int16 PacketType { get; set; }

    public ServerPacketData(string sessionID, byte[] Body, Int16 PacketType)
    {
        this.sessionID = sessionID;
        this.Body = Body;
        this.PacketType = PacketType;
    }
}