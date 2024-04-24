using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace ChattingServer;
public class MainServer : AppServer<ClientSession, EFRequestInfo>
{
    SuperSocket.SocketBase.Config.IServerConfig config = null!;

    UserManager _userManager = new UserManager();
    RoomManager _roomManager = new RoomManager();

    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFRequestInfo>())
    {
        InitConfig();
    }

    public void InitConfig()
    {
        config = new SuperSocket.SocketBase.Config.ServerConfig
        {
            Name = "ChattingServer",
            Port = 7777,
            Ip = "Any",
            MaxConnectionNumber = 100,
            Mode = SuperSocket.SocketBase.SocketMode.Tcp,
        };
    }

    public void CreateStartServer()
    {
        try
        {
            bool bResult = Setup(new SuperSocket.SocketBase.Config.RootConfig(), config);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        if (reason == CloseReason.ClientClosing)
        {
            // Disconnect 처리
        }
    }

    public void OnReceived(ClientSession session, EFRequestInfo requestInfo)
    {
        // 로그인 처리 이후에 User 객체를 생성하고 UserManager에 추가
        // _userManager.AddUser(session, )
        PacketManager.Instance.ParsingPacket(session, requestInfo.Body, (Int16)requestInfo.PacketType);
    }
}