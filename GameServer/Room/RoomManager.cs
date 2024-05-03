using System.Collections.Concurrent;

namespace GameServer;

public class RoomManager
{
    Int32 _maxRoomCount;
    Int32 _maxRoomCheckCount = 0;
    Int32 _nowRoomCheckCount = 0;

    List<Room> _roomPool = new List<Room>();

    public RoomManager(ref readonly ServerOption option)
    {
        _maxRoomCount = option.MaxRoomCount;
        _maxRoomCheckCount = option.MaxRoomCheckCount;

        for (Int32 i = 1; i <= _maxRoomCount; i++)
        {
            _roomPool.Add(new Room(i));
        }
    }

    public Room? GetRoom(Int32 roomID)
    {
        if (roomID < 1 || roomID > _maxRoomCount)
        {
            return null;
        }

        var room = _roomPool.Find(r => r.RoomID == roomID);
        return room;
    }

    public void SetMainServerDelegate(MainServer mainServer)
    {
        foreach (var room in _roomPool)
        {
            room.SetDelegate(mainServer.SendData);
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
        System.Console.WriteLine($"Room Check {_nowRoomCheckCount} {_maxRoomCheckCount} ");

        int maxCount = _nowRoomCheckCount + _maxRoomCheckCount;
        for (; _nowRoomCheckCount < _roomPool.Count; _nowRoomCheckCount++)
        {
            if (_nowRoomCheckCount == maxCount)
                break;

            _roomPool[_nowRoomCheckCount].RoomCheck();
        }

        if (_nowRoomCheckCount >= _maxRoomCount)
        {
            _nowRoomCheckCount = 0;
        }
    }
}