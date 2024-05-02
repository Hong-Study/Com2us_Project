using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;
using Common;

namespace GameServer;

public class PacketRequestInfo : BinaryRequestInfo
{
    public Int16 Size { get; private set; }
    public Int16 PacketType { get; private set; }
    public SByte Type { get; private set; }

    public PacketRequestInfo(Int16 size, Int16 packetType, SByte type, byte[] body)
            : base(null, body)
    {
        this.Size = size;
        this.PacketType = packetType;
        this.Type = type;
    }
}

public class ReceiveFilter : FixedHeaderReceiveFilter<PacketRequestInfo>
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

        var packetSize = FastBinaryRead.Int16(header, offset);
        var bodySize = packetSize - PacketDef.PACKET_HEADER_SIZE;

        return bodySize;
    }

    protected override PacketRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (header.Array == null)
            throw new ArgumentNullException("header.Array");

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(header.Array, 0, PacketDef.PACKET_HEADER_SIZE);

        return new PacketRequestInfo(FastBinaryRead.Int16(header.Array, 0),
                                       FastBinaryRead.Int16(header.Array, 2),
                                       FastBinaryRead.SByte(header.Array, 4),
                                       FastBinaryRead.Bytes(bodyBuffer, offset, length));
    }
}