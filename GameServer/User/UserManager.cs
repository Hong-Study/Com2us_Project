using System.Collections.Concurrent;
using Common;
namespace GameServer;

public class UserManager
{
    int MaxUserCount = 1000;
    ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
    public UserManager(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
    }

    public record AddUserResult(ErrorCode ErrorCode, UserGameData? data);
    public AddUserResult AddUser(string sessionId, UserGameData data)
    {
        if (IsFullUserCount())
        {
            return MakeUserResult(ErrorCode.NONE);
        }

        if (IsExistUser(sessionId))
        {
            return MakeUserResult(ErrorCode.NONE);
        }

        if (IsExistUser(data.user_id))
        {
            return MakeUserResult(ErrorCode.NONE);
        }

        User user = new User(){
            SessionID = sessionId,
            UserID = data.user_id,
            NickName = data.user_name,
            Level = data.level,
            Win = data.win,
            Lose = data.lose,
        };
        
        if (_users.TryAdd(sessionId, user))
        {
            return MakeUserResult(ErrorCode.NONE, data);
        }

        return MakeUserResult(ErrorCode.NONE);
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

    AddUserResult MakeUserResult(ErrorCode errorCode, UserGameData? data = null)
    {
        return new AddUserResult(errorCode, data);
    }
}