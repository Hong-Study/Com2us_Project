using System.Collections.Concurrent;

namespace GameServer;

// 방 관련 기능
public partial class Room : JobQueue
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

        SRoomChatRes res = new SRoomChatRes();
        res.UserName = user.SessionID;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_CHAT);
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
    public bool IsReady { get; set; } = false;
}