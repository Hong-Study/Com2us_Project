using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
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

    public static ILog MainLogger = null!;
    private readonly ILogger<MainServer> _logger;
    private readonly IHostApplicationLifetime _lifeTime;

    public MainServer(IHostApplicationLifetime lifeTime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFRequestInfo>())
    {
        _serverOption = serverConfig.Value;
        _lifeTime = lifeTime;
        _logger = logger;

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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifeTime.ApplicationStarted.Register(AppOnStarted);
        _lifeTime.ApplicationStopping.Register(AppOnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void AppOnStarted()
    {
        _logger.LogInformation("Server OnStart");

        InitConfig(_serverOption);

        CreateStartServer(_serverOption);

        var IsResult = base.Start();
        if (IsResult == false)
        {
            _logger.LogError("Server Start Fail");
        }
        else
        {
            _logger.LogInformation("Server Start Success");
        }
    }

    private void AppOnStopped()
    {
        MainLogger.Info("OnStopped - begin");

        base.Stop();
        IsRunning = false;

        MainLogger.Info("OnStopped - end");
    }

    // 데이터를 모아서 한번에 보내는 방식?
    // 현재는 하나씩 보내는 형식
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

    public void CreateStartServer(ServerOption config)
    {
        try
        {
            bool bResult = Setup(new RootConfig(), _networkConfig, logFactory: new NLogLogFactory());

            if (bResult == false)
            {
                MainLogger.Error("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                return;
            }
            else
            {
                MainLogger = base.Logger;
            }
            IsRunning = true;

            CreateComponent(config);

            MainLogger.Info("서버 생성 성공");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CreateComponent(ServerOption option)
    {
        _packetManager.InitUserDelegate(_userManager);
        _packetManager.InitRoomDelegate(_roomManager);

        _roomManager.Init(SendData);

        _packetManager.Start(1);
    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
        System.Console.WriteLine($"On Connected {session.SessionID} : {session.RemoteEndPoint}");

        // Connect 받았다는 요청 처리
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        System.Console.WriteLine($"On Disconnected {session.SessionID} : {reason}");
        // if (reason == CloseReason.ClientClosing)
        // {
        //     // Disconnect 처리
        // }
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
