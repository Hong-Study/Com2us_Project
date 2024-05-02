using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace GameServer;
public class MainServer : AppServer<ClientSession, PacketRequestInfo>, IHostedService
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

    System.Timers.Timer _sessionTimeoutTimer = new System.Timers.Timer();

    public MainServer(IHostApplicationLifetime lifeTime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, PacketRequestInfo>())
    {
        _serverOption = serverConfig.Value;
        _lifeTime = lifeTime;
        _logger = logger;

        IsRunning = false;

        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnDisconnected);
        NewRequestReceived += new RequestHandler<ClientSession, PacketRequestInfo>(OnReceived);

        _roomManager = new RoomManager(_serverOption.RoomMaxCount);
        _userManager = new UserManager(_serverOption.MaxConnectionNumber);
        _packetManager = new PacketManager();

        _sessionTimeoutTimer.Interval = _serverOption.SessionTimeoutMilliSeconds;
        _sessionTimeoutTimer.Elapsed += (sender, e) =>
        {
            var checkSessionReq = new NTFCheckSessionLoginReq();

            var data = PacketManager.MakeInnerPacket("", checkSessionReq, InnerPacketType.NTF_CHECK_SESSION_LOGIN);
            PacketInnerSend(data);
        };
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
                _logger.LogError("서버 생성 실패");
                return;
            }
            else
            {
                MainLogger = base.Logger;
            }
            IsRunning = true;

            CreateComponent(config);

            MainLogger.Info("서버 생성 성공");

            // _sessionTimeoutTimer.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CreateComponent(ServerOption option)
    {
        _packetManager.SetUserDelegate(_userManager);
        _packetManager.SetRoomDelegate(_roomManager);
        _packetManager.SetMainDelegate(this);

        _roomManager.SetSendDelegate(SendData);

        _packetManager.Start(1);
    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
        MainLogger.Debug($"On Connected {session.SessionID} : {session.RemoteEndPoint}");

        // Connect 받았다는 요청 처리
        // 세션이 로그인 안하는지도 체크해야함.
        session.ConnectionTime = DateTime.Now;
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        MainLogger.Debug($"On Disconnected {session.SessionID} : {reason}");

        var user = _userManager.GetUserInfo(session.SessionID);
        if (user == null)
        {
            return;
        }
        _userManager.RemoveUser(session.SessionID);
        if (user.RoomID != 0)
        {
            var room = _roomManager.GetRoom(user.RoomID);
            if (room == null)
            {
                return;
            }

            room.LeaveRoom(session.SessionID);
        }
    }

    public void OnReceived(ClientSession session, PacketRequestInfo requestInfo)
    {
        // 로그인 처리 이후에 User 객체를 생성하고 UserManager에 추가
        MainLogger.Debug($"On Received {session.SessionID} : {requestInfo.Body.Length}");

        ServerPacketData data = new ServerPacketData(session.SessionID, requestInfo.Body, (Int16)requestInfo.PacketType);

        _packetManager.Distribute(data);
    }

    public void PacketInnerSend(ServerPacketData data)
    {
        _packetManager.Distribute(data);
    }

    public void SessionTimeoutChecked()
    {
        DateTime now = DateTime.Now;
        foreach (var session in GetAllSessions())
        {
            if (session.IsLogin)
            {
                continue;
            }

            TimeSpan span = now - session.ConnectionTime;
            if (span.TotalMilliseconds < _serverOption.SessionTimeoutMilliSeconds)
            {
                continue;
            }

            session.Close();
        }

        _sessionTimeoutTimer.Start();
    }
}