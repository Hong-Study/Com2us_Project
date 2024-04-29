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
    MemoryManager _memoryManager;
    #endregion

    ServerOption _serverOption;
    IServerConfig _networkConfig = null!;
    public static bool IsRunning { get; private set; }

    public static ILog MainLogger = null!;
    private readonly ILogger<MainServer> _logger;
    private readonly IHostApplicationLifetime _lifeTime;

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
        _databaseManager = new DatabaseManager(_serverOption.DatabaseConnectionString);
        _memoryManager = new MemoryManager(_serverOption.MemoryConnectionString);
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
        _packetManager.SetSendDelegate(SendData);

        _roomManager.InitSendDelegate(SendData);

        _packetManager.Start(1);
    }

    public void OnConnected(ClientSession session)
    {
        // 연결 처리
        MainLogger.Debug($"On Connected {session.SessionID} : {session.RemoteEndPoint}");

        // Connect 받았다는 요청 처리
        // 세션이 로그인 안하는지도 체크해야함.
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
        // _userManager.AddUser(session, )
        
        MainLogger.Debug($"On Received {session.SessionID} : {requestInfo.Body.Length}");

        ServerPacketData data = new ServerPacketData(session.SessionID, requestInfo.Body, (Int16)requestInfo.PacketType);

        _packetManager.Distribute(data);
    }

    public void PacketInnerSend(ServerPacketData data)
    {
        _packetManager.Distribute(data);
    }

    public void DatabaseInnerSend(ServerPacketData data)
    {
        _databaseManager.Distribute(data);
    }

    public void MemoryInnerSend(ServerPacketData data)
    {
        _memoryManager.Distribute(data);
    }
}
