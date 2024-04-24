using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;

namespace GameServer;

public class EFRequestInfo : BinaryRequestInfo
{
    public Int16 Size { get; private set; }
    public Int16 PacketType { get; private set; }
    public SByte Type { get; private set; }

    public EFRequestInfo(Int16 size, Int16 packetType, SByte type, byte[] body)
            : base(null, body)
    {
        this.Size = size;
        this.PacketType = packetType;
        this.Type = type;
    }
}

public class ReceiveFilter : FixedHeaderReceiveFilter<EFRequestInfo>
{
    public ReceiveFilter()
        : base(PacketDef.PACKET_HEADER_SIZE)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header, offset, PacketDef.PACKET_HEADER_SIZE);
        }

        var packetSize = BitConverter.ToInt16(header, offset);
        var bodySize = packetSize - PacketDef.PACKET_HEADER_SIZE;

        return bodySize;
    }

    protected override EFRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (header.Array == null)
            throw new ArgumentNullException("header.Array");

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header.Array, 0, PacketDef.PACKET_HEADER_SIZE);

        return new EFRequestInfo(BitConverter.ToInt16(header.Array, 0),
                                       BitConverter.ToInt16(header.Array, 2),
                                       (SByte)header.Array[4],
                                       bodyBuffer.CloneRange(offset, length));
    }
}