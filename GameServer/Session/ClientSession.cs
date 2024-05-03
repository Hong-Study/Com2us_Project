using SuperSocket.SocketBase;

namespace GameServer;

public class ClientSession : AppSession<ClientSession, PacketRequestInfo>
{
    public bool IsLogin { get; set; } = false;
    public DateTime ConnectionTime { get; set; }
}