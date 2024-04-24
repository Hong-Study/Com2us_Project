using SuperSocket.SocketBase;

namespace GameServer;

public class ClientSession : AppSession<ClientSession, EFRequestInfo>
{
    public string RoomNumber { get; set; } = null!;
    public string NickName { get; set; } = null!;
    public bool IsHost { get; set; }
    public User? User { get; set; }
}