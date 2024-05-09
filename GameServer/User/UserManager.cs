using System.Collections.Concurrent;
using Common;
using Microsoft.Extensions.Logging;
namespace GameServer;

public class UserManager
{
    List<User> _users = new List<User>();
    Func<string, byte[], bool> SendFunc = null!;
    Func<string, ClientSession> GetSessionFunc = null!;

    TimeSpan _heartBeatTimeMillisecond;
    TimeSpan _sessionTimeOutMillisecond;

    Int32 _maxUserCount = 0;
    Int32 _nowUserCount = 0;

    Int32 _nowUserPos = 0;

    Int32 _maxHeartBeatCheckCount = 0;
    Int32 _nowHeartBeatCheckCount = 0;

    Int32 _maxSessionCheckCount = 0;
    Int32 _nowSessionCheckCount = 0;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public UserManager(ref readonly ServerOption option)
    {
        _maxUserCount = option.MaxUserCount;
        _maxHeartBeatCheckCount = option.MaxHeartBeatCheckCount;
        _maxSessionCheckCount = option.MaxSessionCheckCount;

        _heartBeatTimeMillisecond = new TimeSpan(0, 0, 0, 0, option.HeartBeatMilliSeconds);
        _sessionTimeOutMillisecond = new TimeSpan(0, 0, 0, 0, option.SessionTimeoutMilliSeconds);

        for (Int32 i = 0; i < _maxUserCount; i++)
        {
            _users.Add(new User());
        }
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
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
            SendResponse<SConnectedRes>(sessionID, ErrorCode.FULL_USER_COUNT, PacketType.RES_S_CONNECT);
            return;
        }

        if (IsExistSession(sessionID))
        {
            SendResponse<SConnectedRes>(sessionID, ErrorCode.ALREADY_EXIST_USER, PacketType.RES_S_CONNECT);
            return;
        }

        try
        {
            while (true)
            {
                if (_nowUserPos == _maxUserCount)
                {
                    _nowUserPos = 0;
                }

                if (_users[_nowUserPos].IsConnect)
                {
                    _nowUserPos += 1;
                    continue;
                }

                _users[_nowUserPos++].SessionConnected(sessionID);
                break;
            }

            var res = new SConnectedRes();
            res.ErrorCode = ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_CONNECT);
            SendFunc(sessionID, bytes);

            _nowUserCount += 1;
        }
        catch
        {
            SendResponse<SConnectedRes>(sessionID, ErrorCode.EXCEPTION_ADD_USER, PacketType.RES_S_CONNECT);
        }
    }

    public void RemoveUser(string sessionID)
    {
        var user = GetUserInfo(sessionID);
        if (user == null)
        {
            SendResponse<SLogOutRes>(sessionID, ErrorCode.NOT_EXIST_USER, PacketType.RES_S_LOGOUT);
            return;
        }

        user.Clear();
        SendResponse<SLogOutRes>(sessionID, ErrorCode.NONE, PacketType.RES_S_LOGOUT);

        _nowUserCount -= 1;
    }

    public void LoginUser(string sessionID, ErrorCode errorCode, UserData? data)
    {
        if (errorCode != ErrorCode.NONE || data == null)
        {
            Logger.Error("LoginUser : Failed Login User");
            SendResponse<SLoginRes>(sessionID, errorCode, PacketType.RES_S_LOGIN);
            SessionDisconnect(sessionID);
            return;
        }

        if (IsExistUser(data.UserID))
        {
            Logger.Error("LoginUser : Already Exist User");
            SendResponse<SLoginRes>(sessionID, ErrorCode.ALREADY_EXIST_USER, PacketType.RES_S_LOGIN);
            SessionDisconnect(sessionID);
            return;
        }

        try
        {
            var user = GetUserInfo(sessionID);
            if (user == null)
            {
                Logger.Error("LoginUser : Not Exist User");
                return;
            }

            user.Logined(data);

            SendResponse<SLoginRes>(sessionID, ErrorCode.NONE, PacketType.RES_S_LOGIN);
        }
        catch
        {
            SessionDisconnect(sessionID);
            SendResponse<SLoginRes>(sessionID, ErrorCode.EXCEPTION_LOGIN_USER, PacketType.RES_S_LOGIN);
        }
    }

    public void LogoutUser(string sessionID)
    {
        var user = GetUserInfo(sessionID);
        if (user == null)
        {
            SendResponse<SLogOutRes>(sessionID, ErrorCode.NOT_EXIST_USER, PacketType.RES_S_LOGOUT);
            return;
        }

        user.Logouted();
        SendResponse<SLogOutRes>(sessionID, ErrorCode.NONE, PacketType.RES_S_LOGOUT);
    }

    public User? GetUserInfo(string sessionID)
    {
        return _users.Find(u => u.SessionID == sessionID);
    }

    public void HeartBeatCheck()
    {
        var now = DateTime.Now;
        Int32 maxCount = _nowHeartBeatCheckCount + _maxHeartBeatCheckCount;
        if (maxCount > _users.Count)
        {
            maxCount = _users.Count;
        }

        for (; _nowHeartBeatCheckCount < maxCount; _nowHeartBeatCheckCount++)
        {
            var user = _users[_nowHeartBeatCheckCount];

            if (!user.IsConnect || !user.IsLogin)
            {
                continue;
            }

            if (now - user.PingTime > _heartBeatTimeMillisecond)
            {
                var session = GetSessionFunc(user.SessionID);
                session.Close();

                user.Clear();
            }
            else
            {
                SendPing(user.SessionID);
            }
        }

        if (_nowHeartBeatCheckCount >= _users.Count)
        {
            _nowHeartBeatCheckCount = 0;
        }
    }

    public void SessionLoginTimeoutCheck()
    {
        var now = DateTime.Now;
        Int32 maxCount = _nowSessionCheckCount + _maxSessionCheckCount;
        if (maxCount > _users.Count)
        {
            maxCount = _users.Count;
        }

        for (; _nowSessionCheckCount < maxCount; _nowSessionCheckCount++)
        {
            var user = _users[_nowSessionCheckCount];

            if (!user.IsConnect || user.IsLogin)
            {
                continue;
            }

            if (now - user.ConnectTime > _sessionTimeOutMillisecond)
            {
                var session = GetSessionFunc(user.SessionID);
                session.Close();

                user.Clear();
            }
        }

        if (_nowSessionCheckCount >= _users.Count)
        {
            _nowSessionCheckCount = 0;
        }
    }

    public void SendPing(string sessionID)
    {
        var req = new SPingReq();
        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_PING);

        SendFunc(sessionID, bytes);
    }

    public void ReceivePong(string sessionID)
    {
        var user = GetUserInfo(sessionID);
        if (user == null)
        {
            return;
        }

        user.PingTime = DateTime.Now;
    }

    void SessionDisconnect(string sessionID)
    {
        var session = GetSessionFunc(sessionID);
        if (session == null)
        {
            return;
        }

        session.Close();
    }

    bool IsFullUserCount()
    {
        return _nowUserCount >= _maxUserCount;
    }

    bool IsExistSession(string sessionID)
    {
        if (_users.Find(u => u.SessionID == sessionID) != null)
        {
            return true;
        }

        return false;
    }

    bool IsExistUser(string userId)
    {
        if (_users.Find(u => u.UserID == userId) != null)
        {
            return true;
        }

        return false;
    }

    void SendResponse<T>(string sessionID, ErrorCode errorCode, PacketType packetType) where T : IResMessage, new()
    {
        if (errorCode != ErrorCode.NONE)
        {
            Logger.Error($"Failed User Action : {errorCode}");
        }

        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, packetType);
        SendFunc(sessionID, bytes);
    }
}