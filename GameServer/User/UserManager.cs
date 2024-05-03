using System.Collections.Concurrent;
using Common;
namespace GameServer;

public class UserManager
{
    List<User> _users = new List<User>();
    Func<string, byte[], bool> SendFunc = null!;
    Func<string, ClientSession> GetSessionFunc = null!;

    TimeSpan _heartBeatTimeMillisecond;
    TimeSpan _sessionTimeOutMillisecond;

    Int32 _maxUserCount = 1000;
    Int32 _nowUserPos = 0;

    Int32 _maxHeartBeatCheckCount = 0;
    Int32 _nowHeartBeatCheckCount = 0;

    Int32 _maxSessionCheckCount = 0;
    Int32 _nowSessionCheckCount = 0;

    public UserManager(ref readonly ServerOption option)
    {
        _maxUserCount = option.MaxUserCount;
        _maxHeartBeatCheckCount = option.MaxHeartBeatCheckCount;
        _maxSessionCheckCount = option.MaxSessionCheckCount;

        _heartBeatTimeMillisecond = new TimeSpan(0, 0, 0, 0, option.HeartBeatMilliSeconds);
        _sessionTimeOutMillisecond = new TimeSpan(0, 0, 0, 0, option.SessionTimeoutMilliSeconds);

        for (int i = 0; i < _maxUserCount; i++)
        {
            _users.Add(new User());
        }
    }

    public void SetMainServerDelegate(MainServer mainServer)
    {
        SendFunc = mainServer.SendData;
        GetSessionFunc = mainServer.GetSessionByID;
    }

    public void AddUser(string sessionID)
    {
        if (IsFullUserCount())
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.FULL_USER_COUNT);
            return;
        }
        else if (IsExistUser(sessionID))
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.ALREADY_EXIST_USER);
            return;
        }

        try
        {
            _users[_nowUserPos++].SessionConnected(sessionID);
            if (_nowUserPos == _maxUserCount)
            {
                _nowUserPos = 0;
            }

            var res = new SLoginRes();
            res.ErrorCode = ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
            SendFunc(sessionID, bytes);
        }
        catch
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.EXCEPTION_ADD_USER);
        }
    }

    public void RemoveUser(string sessionID)
    {
        var user = _users.Find(u => u.sessionID == sessionID);
        if (user == null)
        {
            SendResponse<SLogOutRes>(sessionID, ErrorCode.NOT_EXIST_USER);
            return;
        }

        user.Clear();
        SendResponse<SLogOutRes>(sessionID, ErrorCode.NONE);
    }

    public void LoginUser(string sessionID, UserGameData data)
    {
        if (IsExistUser(data.user_id))
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.ALREADY_EXIST_USER);
            return;
        }

        try
        {
            var user = _users.Find(u => u.IsConfirm(sessionID));
            if (user == null)
            {
                return;
            }

            user.Logined(data);
        }
        catch
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.EXCEPTION_LOGIN_USER);
        }
    }

    public void LogoutUser(string sessionID)
    {
        var user = _users.Find(u => u.sessionID == sessionID);
        if (user == null)
        {
            SendResponse<SLogOutRes>(sessionID, ErrorCode.NOT_EXIST_USER);
            return;
        }

        user.Logouted();
        SendResponse<SLogOutRes>(sessionID, ErrorCode.NONE);
    }

    public User? GetUserInfo(string sessionID)
    {
        return _users.Find(u => u.sessionID == sessionID);
    }

    public void HeartBeatCheck()
    {
        System.Console.WriteLine($"HeartBeatCheck {_nowHeartBeatCheckCount} {_users.Count}");

        var now = DateTime.Now;
        int maxCount = _nowHeartBeatCheckCount + _maxHeartBeatCheckCount;
        for (; _nowHeartBeatCheckCount < _users.Count; _nowHeartBeatCheckCount++)
        {
            if (_nowHeartBeatCheckCount == maxCount)
                break;

            var user = _users[_nowHeartBeatCheckCount];

            if (!user.IsConnect || !user.IsLogin)
            {
                continue;
            }

            if (now - user.PingTime > _heartBeatTimeMillisecond)
            {
                var session = GetSessionFunc(user.sessionID);
                session.Close();

                user.Clear();
            }
        }

        if (_nowHeartBeatCheckCount >= _users.Count)
        {
            _nowHeartBeatCheckCount = 0;
        }
    }

    public void SessionLoginTimeoutCheck()
    {
        System.Console.WriteLine($"SessionLoginTimeoutCheck {_nowSessionCheckCount} {_users.Count}");

        var now = DateTime.Now;
        int maxCount = _nowSessionCheckCount + _maxSessionCheckCount;
        for (; _nowSessionCheckCount < _users.Count; _nowSessionCheckCount++)
        {
            if (_nowSessionCheckCount == maxCount)
                break;

            var user = _users[_nowSessionCheckCount];

            if (!user.IsConnect || user.IsLogin)
            {
                continue;
            }

            if (now - user.ConnectTime > _sessionTimeOutMillisecond)
            {
                var session = GetSessionFunc(user.sessionID);
                session.Close();

                user.Clear();
            }
        }

        if (_nowSessionCheckCount >= _users.Count)
        {
            _nowSessionCheckCount = 0;
        }
    }

    bool IsFullUserCount()
    {
        return _users.Count() >= _maxUserCount;
    }

    bool IsExistUser(string sessionID)
    {
        if (_users.Find(u => u.sessionID == sessionID) != null)
        {
            return true;
        }

        return false;
    }

    bool IsExistUser(Int64 userId)
    {
        if (_users.Find(u => u.UserID == userId) != null)
        {
            return true;
        }

        return false;
    }

    void SendResponse<T>(string sessionID, ErrorCode errorCode) where T : IResMessage, new()
    {
        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionID, bytes);
    }
}