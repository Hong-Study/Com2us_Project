using Common;
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
    DatabaseManager _databaseManager;
    RedisManager _redisManager;
    MatchManager _matchManager;
    #endregion

    ServerOption _serverOption;
    IServerConfig _networkConfig = null!;

    public static bool IsRunning { get; set; }
    public static ILog MainLogger = null!;
    readonly ILogger<MainServer> _logger;
    readonly IHostApplicationLifetime _lifeTime;

    Timer _sessionTimeoutTimer = null!;
    Timer _roomCheckTimer = null!;
    Timer _heartBeatTimer = null!;

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

        _roomManager = new RoomManager(_serverOption);
        _userManager = new UserManager(_serverOption);
        _databaseManager = new DatabaseManager(_serverOption);
        _redisManager = new RedisManager(_serverOption);
        _matchManager = new MatchManager(_serverOption);
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

    void AppOnStarted()
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

    void AppOnStopped()
    {
        MainLogger.Info("OnStopped - begin");

        base.Stop();
        IsRunning = false;


        _matchManager.Stop();

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
            MainLogger.Error("SendData " + ex.Message);

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

            StartTimer();
        }
        catch (Exception ex)
        {
            MainLogger.Error("CreateServer " + ex.Message);
        }
    }

    void CreateComponent(ServerOption option)
    {
        _packetManager.InitLogger(MainLogger);
        _roomManager.InitLogger(MainLogger);
        _userManager.InitLogger(MainLogger);
        _databaseManager.InitLogger(MainLogger);
        _redisManager.InitLogger(MainLogger);
        _matchManager.InitLogger(MainLogger);

        MainServer mainServer = this;

        _packetManager.SetUserDelegate(_userManager);
        _packetManager.SetRoomDelegate(_roomManager);
        _packetManager.SetMainDelegate(mainServer);

        _matchManager.InitUsingRoomList(_roomManager);
        _matchManager.SetMainDelegate(mainServer);

        _roomManager.SetDelegate(SendData, _userManager.GetUserInfo
                                , PacketDatabaseSend, PacketInnerSend, PacketMatchSend);
        _roomManager.SetRoomDefaultSetting(option.OmokGameTurnTimeoutSeconds
                                    , option.OmokGameTurnTimeoutCount
                                    , option.OmokGameMaxGameTimeMinute
                                    , option.MaxMatchingWaitingTimeSeconds);

        _userManager.SetMainServerDelegate(mainServer);

        _databaseManager.SetMainServerDelegate(mainServer);
        _redisManager.SetMainServerDelegate(mainServer);

        _packetManager.Start(1);
        _databaseManager.Start(1);
        _redisManager.Start(1);
        _matchManager.Start();
    }

    void StartTimer()
    {
        StartRoomCheckTimer();
        SessionCheckTimer();
        HeartBeatTimerTimer();
    }

    void HeartBeatTimerTimer()
    {
        TimerCallback sessionTimeoutCallback = (object? state) =>
        {
            var checkSessionReq = new NTFHeartBeatReq();

            var data = PacketManager.MakeInnerPacket("", checkSessionReq, InnerPacketType.NTF_HEART_BEAT);
            PacketInnerSend(data);
        };

        TimeSpan period = TimeSpan.FromMilliseconds(_serverOption.HeartBeatTimerMilliSeconds);
        _heartBeatTimer = new Timer(sessionTimeoutCallback, null, period, period);
    }

    void StartRoomCheckTimer()
    {
        TimerCallback sessionTimeoutCallback = (object? state) =>
        {
            var checkSessionReq = new NTFRoomsCheckReq();

            var data = PacketManager.MakeInnerPacket("", checkSessionReq, InnerPacketType.NTF_ROOMS_CHECK);
            PacketInnerSend(data);
        };

        TimeSpan period = TimeSpan.FromMilliseconds(_serverOption.RoomCheckTimerMilliSeconds);
        _roomCheckTimer = new Timer(sessionTimeoutCallback, null, period, period);
    }

    void SessionCheckTimer()
    {
        TimerCallback sessionTimeoutCallback = (object? state) =>
        {
            var checkSessionReq = new NTFCheckSessionLoginReq();

            var data = PacketManager.MakeInnerPacket("", checkSessionReq, InnerPacketType.NTF_CHECK_SESSION_LOGIN);
            PacketInnerSend(data);
        };

        TimeSpan period = TimeSpan.FromMilliseconds(_serverOption.SessionTimeoutTimerMilliSeconds);
        _sessionTimeoutTimer = new Timer(sessionTimeoutCallback, null, period, period);
    }

    public void OnConnected(ClientSession session)
    {
        MainLogger.Debug($"On Connected {session.SessionID} : {session.RemoteEndPoint}");

        var packet = new NTFSessionConnectedReq();
        packet.SessionID = session.SessionID;

        var data = PacketManager.MakeInnerPacket(session.SessionID, packet, InnerPacketType.NTF_SESSION_CONNECTED);
        PacketInnerSend(data);
    }

    public void OnDisconnected(ClientSession session, CloseReason reason)
    {
        MainLogger.Debug($"On Disconnected {session.SessionID} : {reason}");

        var packet = new NTFSessionDisconnectedReq();
        packet.SessionID = session.SessionID;

        var data = PacketManager.MakeInnerPacket(session.SessionID, packet, InnerPacketType.NTF_SESSION_DISCONNECTED);
        PacketInnerSend(data);
    }

    public void OnReceived(ClientSession session, PacketRequestInfo requestInfo)
    {
        ServerPacketData data = new ServerPacketData(session.SessionID, requestInfo.Body, (Int16)requestInfo.PacketType);

        _packetManager.Distribute(data);
    }

    public void PacketInnerSend(ServerPacketData data)
    {
        if (data.PacketType > (Int16)InnerPacketType.INNER_PACKET_START && data.PacketType < (Int16)InnerPacketType.INNER_PACKET_END)
        {
            _packetManager.Distribute(data);
        }
    }

    public void PacketDatabaseSend(ServerPacketData data)
    {
        if (data.PacketType > (Int16)DatabaseType.DB_PACKET_START && data.PacketType < (Int16)DatabaseType.DB_PACKET_END)
        {
            _databaseManager.Distribute(data);
        }
    }

    public void PacketRedisSend(ServerPacketData data)
    {
        if (data.PacketType > (Int16)RedisType.REDIS_PACKET_START && data.PacketType < (Int16)RedisType.REDIS_PACKET_END)
        {
            _redisManager.Distribute(data);
        }
    }

    public void PacketMatchSend(byte[] data)
    {
        _matchManager.Distribute(data);
    }
}