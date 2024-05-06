using System.Collections.Concurrent;
using Common;
using Microsoft.Extensions.Logging;
namespace GameServer;

public class UserManager
{
    List<User> _users = new List<User>();
    Func<string, byte[], bool> SendFunc = null!;
    Func<string, ClientSession> GetSessionFunc = null!;

    Action<ServerPacketData> DatabaseSendFunc = null!;

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
            SendResponse<SConnectedRes>(sessionID, ErrorCode.FULL_USER_COUNT);
            return;
        }
        else if (IsExistUser(sessionID))
        {
            SendResponse<SConnectedRes>(sessionID, ErrorCode.ALREADY_EXIST_USER);
            return;
        }

        try
        {
            _users[_nowUserPos++].SessionConnected(sessionID);
            if (_nowUserPos == _maxUserCount)
            {
                _nowUserPos = 0;
            }

            var res = new SConnectedRes();
            res.ErrorCode = ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
            SendFunc(sessionID, bytes);

            _nowUserCount += 1;
        }
        catch
        {
            SendResponse<SConnectedRes>(sessionID, ErrorCode.EXCEPTION_ADD_USER);
        }
    }

    public void RemoveUser(string sessionID)
    {
        var user = GetUserInfo(sessionID);
        if (user == null)
        {
            SendResponse<SLogOutRes>(sessionID, ErrorCode.NOT_EXIST_USER);
            return;
        }

        user.Clear();
        SendResponse<SLogOutRes>(sessionID, ErrorCode.NONE);

        _nowUserCount -= 1;
    }

    public void LoginUser(string sessionID, ErrorCode errorCode, UserData? data)
    {
        if (errorCode != ErrorCode.NONE)
        {
            SendResponse<SLoginRes>(sessionID, errorCode);
            return;
        }

        if (data == null)
        {
            SendResponse<SLoginRes>(sessionID, errorCode);
            return;
        }

        if (IsExistUser(data.UserID))
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.ALREADY_EXIST_USER);
            return;
        }

        try
        {
            var user = GetUserInfo(sessionID);
            if (user == null)
            {
                return;
            }

            user.Logined(data);

            SendResponse<SLoginRes>(sessionID, ErrorCode.NONE);
        }
        catch
        {
            SendResponse<SLoginRes>(sessionID, ErrorCode.EXCEPTION_LOGIN_USER);
        }
    }

    public void LogoutUser(string sessionID)
    {
        var user = GetUserInfo(sessionID);
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

    public void UpdateUserWinLoseCount(string sessionID, bool isWin)
    {
        var user = GetUserInfo(sessionID);
        if (user == null)
        {
            return;
        }

        if (isWin)
        {
            user.Win += 1;
        }
        else
        {
            user.Lose += 1;
        }

        var req = new DBUpdateWinLoseCountReq();
        req.UserID = user.UserID;
        req.WinCount = user.Win;
        req.LoseCount = user.Lose;

        var data = DatabaseManager.MakeDatabasePacket(sessionID, req, DatabaseType.REQ_DB_UPDATE_WIN_LOSE_COUNT);
        DatabaseSendFunc(data);
    }

    bool IsFullUserCount()
    {
        return _nowUserCount >= _maxUserCount;
    }

    bool IsExistUser(string sessionID)
    {
        if (_users.Find(u => u.SessionID == sessionID) != null)
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
        Logger.Error($"Failed User Action : {errorCode}");

        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_LOGIN);
        SendFunc(sessionID, bytes);
    }
}