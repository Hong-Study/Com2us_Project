using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace GameServer;
public class MainServer : AppServer<ClientSession, EFRequestInfo>, IHostedService
{
    #region Managers
    RoomManager _roomManager;
    UserManager _userManager;
    PacketManager _packetManager;
    #endregion

    ServerOption _serverOption;
    IServerConfig _networkConfig = null!;
    public static bool IsRunning { get; private set; }

    private readonly IHostApplicationLifetime _lifeTime;

    public MainServer(IHostApplicationLifetime lifeTime, IOptions<ServerOption> options)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFRequestInfo>())
    {
        _serverOption = options.Value;
        _lifeTime = lifeTime;
        IsRunning = false;

        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnDisconnected);
        NewRequestReceived += new RequestHandler<ClientSession, EFRequestInfo>(OnReceived);

        _roomManager = new RoomManager(_serverOption.RoomMaxCount);
        _userManager = new UserManager(_serverOption.MaxConnectionNumber);
        _packetManager = new PacketManager();
    }

    public void InitConfig(ServerOption option)
    {
        _networkConfig = new ServerConfig
        {
            Name = option.Name,
            Port = option.Port,
            Ip = "Any",
            Mode = SocketMode.Tcp,
            MaxConnectionNumber = option.MaxConnectionNumber,
            MaxRequestLength = option.MaxRequestLength,
            ReceiveBufferSize = option.ReceiveBufferSize,
            SendBufferSize = option.SendBufferSize,
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _lifeTime.ApplicationStarted.Register(AppOnStarted);
        _lifeTime.ApplicationStopping.Register(AppOnStopped);

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private void AppOnStarted()
    {
        InitConfig(_serverOption);

        CreateStartServer(_serverOption);

        var IsResult = base.Start();
        if (IsResult == false)
        {
            Console.WriteLine("Server Start Fail");
        }
        else
        {
            Console.WriteLine("Server Start Success");
        }
    }

    private void AppOnStopped()
    {
        base.Stop();
    }

    public bool SendData(string sessionID, byte[] bytes)
    {
        var session = GetSessionByID(sessionID);

        try
        {
            if (session == null)
            {
                return false;
            }

            session.Send(bytes, 0, bytes.Length);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            session.SendEndWhenSendingTimeOut();
            session.Close();
            return false;
        }
    }

    public void CreateStartServer(ServerOption option)
    {
        try
        {
            // 서버 팩토리 없는 버전
            bool bResult = Setup(_networkConfig);

            if (bResult == false)
            {
                Console.WriteLine("Server Setup Fail");
                return;
            }

            IsRunning = true;

            CreateComponent(option);

            Start();
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

    private void CreateComponent(ServerOption option)
    {
        _packetManager.Start(1);

        _packetManager.InitUserDelegate(_userManager);
        _packetManager.InitRoomDelegate(_roomManager);
    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
        System.Console.WriteLine($"On Connected {session.SessionID} : {session.RemoteEndPoint}");

        // Connect 받았다는 요청 처리
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        if (reason == CloseReason.ClientClosing)
        {
            // Disconnect 처리
            System.Console.WriteLine($"On Disconnected {session.SessionID} : {reason}");
        }
    }

    public void OnReceived(ClientSession session, EFRequestInfo requestInfo)
    {
        // 로그인 처리 이후에 User 객체를 생성하고 UserManager에 추가
        // _userManager.AddUser(session, )
        System.Console.WriteLine($"On Received {session.SessionID} : {requestInfo.Body.Length}");

        ServerPacketData data = new ServerPacketData(session.SessionID, requestInfo.Body, (Int16)requestInfo.PacketType);

        _packetManager.Distribute(data);
    }
}
