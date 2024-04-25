using System.Collections.Concurrent;

namespace GameServer;

public class UserManager
{
    int MaxUserCount = 1000;
    ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

    public UserManager(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
    }

    public ErrorCode AddUser(string sessionId, UserGameData data)
    {
        if (IsFullUserCount())
        {
            return ErrorCode.NONE;
        }

        if (IsExistUser(sessionId))
        {
            return ErrorCode.NONE;
        }

        if (IsExistUser(data.user_id))
        {
            return ErrorCode.NONE;
        }

        User user = new User(sessionId);
        _users.TryAdd(sessionId, user);

        return ErrorCode.NONE;
    }

    public ErrorCode RemoveUser(string sessionId)
    {
        if (_users.TryRemove(sessionId, out User? user))
        {
            user.Clear();
            return ErrorCode.NONE;
        }
        else
        {
            return ErrorCode.NONE;
        }
    }

    public User? GetUserInfo(string sessionId)
    {
        if (_users.TryGetValue(sessionId, out User? user))
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    bool IsFullUserCount()
    {
        return _users.Count() >= MaxUserCount;
    }

    bool IsExistUser(string sessionId)
    {
        return _users.ContainsKey(sessionId);
    }

    bool IsExistUser(Int64 userId)
    {
        return _users.Values.Any(user => user.UserID == userId);
    }
}