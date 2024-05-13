using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using CloudStructures.Structures;
using StackExchange.Redis;

namespace GameServer;

public class MatchManager
{
    Func<Int32, bool> SetRoomStateEmptyFunc = null!;
    Func<Int32, bool> SetRoomStateWatingFunc = null!;
    Func<Int32, bool> SetRoomStateMathcingFunc = null!;

    Func<UsingRoomInfo?> GetEmptyRoomInfoFunc = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    RedisConnector _redisMatchConnection = null!;
    RedisConnector _redisCompleteConnection = null!;
    RedisList<string> _matchList;
    RedisList<string> _completeList;

    string _serverAddress = "";
    Int32 _serverPort = 0;

    public MatchManager(ref readonly ServerOption option)
    {
        _redisMatchConnection = new RedisConnector(option.RedisConnectionString);
        _redisCompleteConnection = new RedisConnector(option.RedisConnectionString);

        RedisKey matchKey = option.RedisSubKey;
        RedisKey completeKey = option.RedisPubKey;

        TimeSpan timeout = TimeSpan.MaxValue;

        _matchList = new RedisList<string>(_redisMatchConnection.RedisCon, matchKey, timeout);
        _completeList = new RedisList<string>(_redisCompleteConnection.RedisCon, completeKey, timeout);

        _serverAddress = "127.0.0.1";
        _serverPort = option.Port;
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void SetRoomDelegate(ref readonly RoomManager roomManager)
    {
        SetRoomStateEmptyFunc = roomManager.SetRoomStateEmpty;
        SetRoomStateMathcingFunc = roomManager.SetRoomStateMathcing;
        SetRoomStateWatingFunc = roomManager.SetRoomStateWaiting;

        GetEmptyRoomInfoFunc = roomManager.GetEmptyRoom;
    }

    public void Start()
    {
        // _logicThread = new Thread(Process);
        // _logicThread.Start();

        Process();
    }

    async void Process()
    {
        while (MainServer.IsRunning)
        {
            try
            {
                if(RoomManager.IsExistEmptyRoom == false)
                {
                    Thread.Sleep(10000);
                    continue;
                }

                var message = await _matchList.RightPopAsync();
                if (message.HasValue == false)
                {
                    continue;
                }

                System.Console.WriteLine("MatchManager Process");

                await OnMatchingHandle(message.Value);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }

    async Task OnMatchingHandle(string result)
    {
        var matchingData = JsonSerializer.Deserialize<MatchingData>(result);
        if(matchingData == null)
        {
            System.Console.WriteLine("MatchingData is null");
            return;
        }

        var roomInfo = GetEmptyRoomInfoFunc();
        
        if (roomInfo == null)
        {
            // 빈 방이 없으니 Subscribe를 종료하거나? Sleep을 한다.
            System.Console.WriteLine("Empty Room is not exist");
            return;
        }

        bool isSuccess = SetRoomStateMathcingFunc(roomInfo.RoomID);
        if(isSuccess == false)
        {
            System.Console.WriteLine("SetRoomStateMathcingFunc Fail");
            return;
        }

        if(matchingData.MatchingUserData == null)
        {
            System.Console.WriteLine("MatchingUserData is null");
            return;
        }

        System.Console.WriteLine("Matching Success");

        var completeMatchingData = new CompleteMatchingData();
        completeMatchingData.MatchID = matchingData.MatchID;
        completeMatchingData.FirstUserID = matchingData.MatchingUserData.FirstUserID;
        completeMatchingData.SecondUserID = matchingData.MatchingUserData.SecondUserID;
        completeMatchingData.ServerInfo = new MatchingServerInfo()
        {
            ServerAddress = _serverAddress,
            Port = _serverPort,
            RoomNumber = roomInfo.RoomID
        };

        await _completeList.LeftPushAsync(JsonSerializer.Serialize(completeMatchingData));
    }
}