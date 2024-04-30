using System.Collections.Concurrent;
using Common;
namespace GameServer;

public class UserManager
{
    int MaxUserCount = 1000;
    ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
    Func<string, byte[], bool> SendFunc = null!;

    public UserManager(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
    }

    public void AddUser(string sessionId, UserGameData data)
    {
        if (IsFullUserCount())
        {
            SendFailedLoginResult(sessionId, ErrorCode.FULL_USER_COUNT);
            return;
        }
        else if (IsExistUser(sessionId) || IsExistUser(data.user_id))
        {
            SendFailedLoginResult(sessionId, ErrorCode.ALREADY_EXIST_USER);
            return;
        }

        User user = new User()
        {
            SessionID = sessionId,
            UserID = data.user_id,
            NickName = data.user_name,
            Level = data.level,
            Win = data.win,
            Lose = data.lose,
        };

        try
        {
            _users.TryAdd(sessionId, user);
            var res = new SLoginRes();
            res.ErrorCode = ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
            SendFunc(sessionId, bytes);
        }
        catch
        {
            SendFailedLoginResult(sessionId, ErrorCode.ADD_USER_EXCEPTION);
        }
    }

    public void RemoveUser(string sessionId)
    {
        if (_users.TryRemove(sessionId, out User? user))
        {
            user.Clear();

            SendFailedLoginResult(sessionId, ErrorCode.NONE);
        }
        else
        {
            SendFailedLoginResult(sessionId, ErrorCode.NOT_EXIST_USER);
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

    void SendFailedLoginResult(string sessionId, ErrorCode errorCode)
    {
        var res = new SLoginRes();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionId, bytes);
    }

    void SendFailedLoginResult(string sessionId, ErrorCode errorCode, string message)
    {
        var res = new SLoginRes();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionId, bytes);
    }
}