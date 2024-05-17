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
            _roomPool.Add(new Room(i, _roomStartNumber + i));
        }
    }

    public void InitUsingRoomList(ref List<UsingRoomInfo> usingRoomInfos)
    {
        foreach (var room in _roomPool)
        {
            UsingRoomInfo usingRoomInfo = new UsingRoomInfo();
            usingRoomInfo.RoomNumber = room.RoomNumber;
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

    public Room? GetRoomID(Int32 roomID)
    {
        if (roomID < 0 || roomID > _maxRoomCount)
        {
            return null;
        }

        return _roomPool[roomID];
    }

    public Room? GetRoomNumber(Int32 roomNumber)
    {
        if (roomNumber < _roomStartNumber || roomNumber >= _roomStartNumber + _maxRoomCount)
        {
            return null;
        }

        return _roomPool.Find(r => r.RoomNumber == roomNumber);
    }

    public void SetDelegate(Func<string, byte[], bool> SendFunc, Func<string, User?> GetUserInfoFunc
                            , Action<ServerPacketData> databaseSendFunc
                            , Action<ServerPacketData> sendInnerFunc
                            , Action<byte[]> matchInnerFunc)
    {
        foreach (var room in _roomPool)
        {
            room.SetDelegate(SendFunc, GetUserInfoFunc, databaseSendFunc, sendInnerFunc, matchInnerFunc);
        }
    }

    public void SetDefaultSetting(Int32 turnTimeoutSecond, Int32 timeoutCount
                                , Int32 maxGameTimeMinute, Int32 maxMatchingWaitingTimeSecond)
    {
        foreach (var room in _roomPool)
        {
            room.InitDefaultSetting(turnTimeoutSecond, timeoutCount, maxGameTimeMinute, maxMatchingWaitingTimeSecond);
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