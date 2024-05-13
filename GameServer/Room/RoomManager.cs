using System.Collections.Concurrent;

namespace GameServer;

public class RoomManager
{
    Int32 _maxRoomCount;
    Int32 _maxRoomCheckCount = 0;
    Int32 _nowRoomCheckCount = 1;

    List<Room> _roomPool = new List<Room>();
    List<UsingRoomInfo> _roomUsingInfoList = new List<UsingRoomInfo>();

    public static bool IsExistEmptyRoom { get => Interlocked.CompareExchange(ref _isExistEmptyRoom, 1, 1) == 1; }
    static Int32 _isExistEmptyRoom = 1;
    Int32 _roomUsingCount = 0;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    object _lock = new object();

    public RoomManager(ref readonly ServerOption option)
    {
        _maxRoomCount = option.MaxRoomCount;
        _maxRoomCheckCount = option.MaxRoomCheckCount;

        _roomPool.Add(new Room(0));
        for (Int32 i = 1; i <= _maxRoomCount; i++)
        {
            _roomPool.Add(new Room(i));
            _roomUsingInfoList.Add(new UsingRoomInfo() { RoomID = i, RoomState = RoomState.Empty });
        }
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        foreach (var room in _roomPool)
        {
            room.InitLogger(logger);
        }
    }

    public UsingRoomInfo? GetEmptyRoom()
    {
        lock (_lock)
        {
            foreach (var roomInfo in _roomUsingInfoList)
            {
                if (roomInfo.RoomState == RoomState.Empty)
                {
                    Interlocked.Exchange(ref _isExistEmptyRoom, 1);
                    return roomInfo;
                }
            }

            Interlocked.Exchange(ref _isExistEmptyRoom, 0);

            return null;
        }
    }

    public RoomState GetRoomState(Int32 roomID)
    {
        lock (_lock)
        {
            if (roomID < 1 || roomID > _maxRoomCount)
            {
                return RoomState.Empty;
            }

            return _roomUsingInfoList[roomID - 1].RoomState;
        }
    }

    public bool SetRoomStateEmpty(Int32 roomID)
    {
        lock (_lock)
        {
            if (roomID < 1 || roomID > _maxRoomCount)
            {
                return false;
            }

            _roomUsingInfoList[roomID - 1].RoomState = RoomState.Empty;
        }

        Interlocked.Add(ref _roomUsingCount, 1);

        return true;
    }

    public bool SetRoomStateMathcing(Int32 roomID)
    {
        lock (_lock)
        {
            if (roomID < 1 || roomID > _maxRoomCount)
            {
                return false;
            }

            _roomUsingInfoList[roomID - 1].RoomState = RoomState.Mathcing;
        }

        return true;
    }

    public bool SetRoomStateWaiting(Int32 roomID)
    {
        lock (_lock)
        {
            if (roomID < 1 || roomID > _maxRoomCount)
            {
                return false;
            }

            if (_roomUsingInfoList[roomID - 1].RoomState != RoomState.Empty)
            {
                return false;
            }

            _roomUsingInfoList[roomID - 1].RoomState = RoomState.Waiting;
        }

        Interlocked.Add(ref _roomUsingCount, 1);

        return true;
    }

    public Room? GetRoom(Int32 roomID)
    {
        if (roomID < 1 || roomID > _maxRoomCount)
        {
            return null;
        }

        return _roomPool.Find(r => r.RoomID == roomID);
    }

    public void SetDelegate(Func<string, byte[], bool> SendFunc, Func<string, User?> GetUserInfoFunc
                            , Action<ServerPacketData> databaseSendFunc)
    {
        foreach (var room in _roomPool)
        {
            room.SetDelegate(SendFunc, GetUserInfoFunc, databaseSendFunc);
        }
    }

    public void SetDefaultSetting(Int32 turnTimeoutSecond, Int32 timeoutCount, Int32 maxGameTimeMinute)
    {
        foreach (var room in _roomPool)
        {
            room.InitDefaultSetting(turnTimeoutSecond, timeoutCount, maxGameTimeMinute);
        }
    }

    public void AddUsingRoomCount()
    {
        _roomUsingCount++;

        if (_roomUsingCount == _maxRoomCount)
        {
            Interlocked.Exchange(ref _isExistEmptyRoom, 0);
        }
    }

    public void SubUsingRoomCount()
    {
        _roomUsingCount--;

        if (_roomUsingCount < _maxRoomCount)
        {
            Interlocked.Exchange(ref _isExistEmptyRoom, 1);
        }
    }

    public void RoomsCheck()
    {
        Int32 maxCount = _nowRoomCheckCount + _maxRoomCheckCount;
        if (maxCount > _maxRoomCount)
        {
            maxCount = _maxRoomCount + 1;
        }
        
        for (; _nowRoomCheckCount < maxCount; _nowRoomCheckCount++)
        {
            _roomPool[_nowRoomCheckCount].RoomCheck();
        }

        if (_nowRoomCheckCount >= _maxRoomCount)
        {
            _nowRoomCheckCount = 1;
        }
    }
}

public class UsingRoomInfo
{
    public Int32 RoomID { get; set; }
    public RoomState RoomState { get; set; } = RoomState.Empty;
}

public enum RoomState
{
    NONE = 0,
    Empty,
    Waiting,
    Mathcing
}