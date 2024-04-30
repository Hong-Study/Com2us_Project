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
            SendResponse<SLoginRes>(sessionId, ErrorCode.FULL_USER_COUNT);
            return;
        }
        else if (IsExistUser(sessionId) || IsExistUser(data.user_id))
        {
            SendResponse<SLoginRes>(sessionId, ErrorCode.ALREADY_EXIST_USER);
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
            SendResponse<SLoginRes>(sessionId, ErrorCode.ADD_USER_EXCEPTION);
        }
    }

    public void RemoveUser(string sessionId)
    {
        if (_users.TryRemove(sessionId, out User? user))
        {
            user.Clear();

            SendResponse<SLogOutRes>(sessionId, ErrorCode.NONE);
        }
        else
        {
            SendResponse<SLogOutRes>(sessionId, ErrorCode.NOT_EXIST_USER);
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

    void SendResponse<T>(string sessionID, ErrorCode errorCode) where T : IResMessage, new()
    {
        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionID, bytes);
    }
}