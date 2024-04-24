using System.Collections.Concurrent;

namespace ChattingServer;

public class UserManager
{
    UInt64 UserSequenceNumber = 0;
    int MaxUserCount = 0;
    ConcurrentDictionary<UInt64, User> _users = new ConcurrentDictionary<UInt64, User>();

    public User? AddUser(ClientSession session, UserGameData data)
    {
        if (IsFullUserCount())
        {
            return null;
        }

        var id = Interlocked.Increment(ref UserSequenceNumber);

        User user = new User(session);
        _users.TryAdd(id, user);
        return user;
    }

    public bool RemoveUser(UInt64 id)
    {
        if (_users.TryRemove(id, out User? user))
        {
            user.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFullUserCount()
    {
        return _users.Count() >= MaxUserCount;
    }
}