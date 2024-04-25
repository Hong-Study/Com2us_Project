using System.Collections.Concurrent;

namespace GameServer;

public class RoomManager
{
    int MaxRoomCount;
    ConcurrentDictionary<int, Room> _roomPool = new ConcurrentDictionary<int, Room>();

    public RoomManager(int maxRoomCount)
    {
        MaxRoomCount = maxRoomCount;

        for (int i = 1; i <= MaxRoomCount; i++)
        {
            _roomPool.TryAdd(i, new Room(i));
        }
    }

    public Room? GetRoom(int roomNumber)
    {
        if(roomNumber < 1 || roomNumber > MaxRoomCount)
        {
            return null;
        }
        
        _roomPool.TryGetValue(roomNumber, out Room? room);

        return room;
    }
}