using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace GameServer;
public class MainServer : AppServer<ClientSession, EFRequestInfo>
{
    SuperSocket.SocketBase.Config.IServerConfig config = null!;
    public static bool IsRunning { get; private set; }
    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFRequestInfo>())
    {
        IsRunning = false;
    }

    public void InitConfig(string IP, int port)
    {
        config = new SuperSocket.SocketBase.Config.ServerConfig
        {
            Name = "GameServer",
            Port = port,
            Ip = IP,
        };
    }

    public void CreateStartServer()
    {
        try
        {
            // 서버 팩토리 없는 버전
            bool bResult = Setup(config);

            if (bResult == false)
            {
                Console.WriteLine("Server Setup Fail");
                return;
            }

            CreateComponent();

            Start();

            IsRunning = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void StopServer()
    {
        Stop();
        IsRunning = false;
    }

    private void CreateComponent()
    {

    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
        System.Console.WriteLine("On Connected");
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        if (reason == CloseReason.ClientClosing)
        {
            // Disconnect 처리
            System.Console.WriteLine("On Connected");
        }
    }

    public void OnReceived(ClientSession session, EFRequestInfo requestInfo)
    {
        // 로그인 처리 이후에 User 객체를 생성하고 UserManager에 추가
        // _userManager.AddUser(session, )
        ServerPacketData data = new ServerPacketData(session, requestInfo.Body, (Int16)requestInfo.PacketType);
        data.Session = session;
        data.Body = requestInfo.Body;
        data.PacketType = requestInfo.PacketType;

        PacketManager.Instance.Distribute(data);
    }
}
