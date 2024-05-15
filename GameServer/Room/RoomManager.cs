using System.Collections.Concurrent;

namespace GameServer;

public class RoomManager
{
    Int32 _roomStartNumber = 1;
    Int32 _maxRoomCount;
    Int32 _maxRoomCheckCount = 0;
    Int32 _nowRoomCheckCount = 0;

    List<Room> _roomPool = new List<Room>();

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    object _lock = new object();

    public RoomManager(ref readonly ServerOption option)
    {
        _maxRoomCount = option.MaxRoomCount;
        _maxRoomCheckCount = option.MaxRoomCheckCount;
        _roomStartNumber = option.RoomStartNumber;

        for (Int32 i = 0; i < _maxRoomCount; i++)
        {
            _roomPool.Add(new Room(i + _roomStartNumber));
        }
    }

    public void InitUsingRoomList(ref List<UsingRoomInfo> usingRoomInfos)
    {
        foreach (var room in _roomPool)
        {
            UsingRoomInfo usingRoomInfo = new UsingRoomInfo();
            usingRoomInfo.RoomID = room.RoomID;
            usingRoomInfo.RoomState = RoomState.Empty;

            usingRoomInfos.Add(usingRoomInfo);
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

    public Room? GetRoom(Int32 roomID)
    {
        if (roomID < _roomStartNumber || roomID >= _maxRoomCount + _roomStartNumber)
        {
            return null;
        }

        return _roomPool.Find(r => r.RoomID == roomID);
    }

    public void SetDelegate(Func<string, byte[], bool> SendFunc, Func<string, User?> GetUserInfoFunc
                            , Action<ServerPacketData> databaseSendFunc
                            , Action<ServerPacketData> sendInnerFunc)
    {
        foreach (var room in _roomPool)
        {
            room.SetDelegate(SendFunc, GetUserInfoFunc, databaseSendFunc, sendInnerFunc);
        }
    }

    public void SetDefaultSetting(Int32 turnTimeoutSecond, Int32 timeoutCount, Int32 maxGameTimeMinute)
    {
        foreach (var room in _roomPool)
        {
            room.InitDefaultSetting(turnTimeoutSecond, timeoutCount, maxGameTimeMinute);
        }
    }

    public void RoomsCheck()
    {
        Int32 maxCount = _nowRoomCheckCount + _maxRoomCheckCount;
        if (maxCount > _maxRoomCount)
        {
            maxCount = _maxRoomCount;
        }

        for (; _nowRoomCheckCount < maxCount; _nowRoomCheckCount++)
        {
            _roomPool[_nowRoomCheckCount].RoomCheck();
        }

        if (_nowRoomCheckCount >= _maxRoomCount)
        {
            _nowRoomCheckCount = 0;
        }
    }
}