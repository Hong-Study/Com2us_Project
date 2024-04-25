using System.Collections.Concurrent;

namespace GameServer;

public class Room
{
    ConcurrentDictionary<string, RoomUser> _users = new ConcurrentDictionary<string, RoomUser>();

    public Func<string, byte[], bool> SendFunc = null!;
    public Int32 RoomID { get; private set; }

    public Room(Int32 roomId)
    {
        RoomID = roomId;
    }

    public bool EnterRoom(User user)
    {
        RoomUser roomUser = new RoomUser();
        roomUser.SessionID = user.SessionID;
        roomUser.UserID = user.UserID;

        try
        {
            _users.TryAdd(user.SessionID, roomUser);
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
    }

    public bool LeaveRoom(string sessionID)
    {
        if (_users.TryRemove(sessionID, out RoomUser? user) == false)
        {
            return false;
        }

        return true;
    }

    public void SendChat(string sessionID, string message)
    {
        // 가져오는 코드
        if (_users.TryGetValue(sessionID, out RoomUser? user) == false)
        {
            return;
        }

        RoomChatRes res = new RoomChatRes();
        res.UserName = user.SessionID;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.ROOM_CHAT);
        BroadCast(bytes);
    }

    void BroadCast(byte[] bytes, string expiredSessionID = "")
    {
        foreach (var (id, user) in _users)
        {
            if (id == expiredSessionID)
            {
                continue;
            }

            SendFunc(id, bytes);
        }
    }
}

public class RoomUser
{
    public string SessionID { get; set; } = null!;
    public Int64 UserID { get; set; }
}