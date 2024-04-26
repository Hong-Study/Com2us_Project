using System;

namespace GameClient;

public class ClientPacketData
{
    public Int16 Size { get; private set; }
    public Int16 PacketType { get; private set; }
    public SByte Type { get; private set; }

    public ArraySegment<byte> Body { get; private set; }

    public ClientPacketData(byte[] bytes)
    {
        Size = BitConverter.ToInt16(bytes, 0);
        PacketType = BitConverter.ToInt16(bytes, 2);
        Type = (SByte)bytes[4];

        Body = new ArraySegment<byte>(bytes, PacketDef.PACKET_HEADER_SIZE, Size - PacketDef.PACKET_HEADER_SIZE);
    }
}
