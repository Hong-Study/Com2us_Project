using System.Collections.Concurrent;

namespace GameServer;

public class RoomManager
{
    Int32 _maxRoomCount;
    Int32 _maxRoomCheckCount = 0;
    Int32 _nowRoomCheckCount = 0;

    List<Room> _roomPool = new List<Room>();

    SuperSocket.SocketBase.Logging.ILog Logger = null!;
    
    public RoomManager(ref readonly ServerOption option)
    {
        _maxRoomCount = option.MaxRoomCount;
        _maxRoomCheckCount = option.MaxRoomCheckCount;

        for (Int32 i = 1; i <= _maxRoomCount; i++)
        {
            _roomPool.Add(new Room(i));
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

    public void RoomsCheck()
    {
        Int32 maxCount = _nowRoomCheckCount + _maxRoomCheckCount;
        if (maxCount > _roomPool.Count)
        {
            maxCount = _roomPool.Count;
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