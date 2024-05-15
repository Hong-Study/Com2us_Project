using Common;

namespace GameServer;

public class Room
{
    public Int32 RoomID { get; private set; }
    public RoomState State { get; private set; }

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    List<RoomUser> _users = new List<RoomUser>();
    List<string> _matchingUsers = new List<string>();
    Func<string, byte[], bool> SendFunc = null!;
    Func<string, User?> GetUserInfoFunc = null!;

    Action<ServerPacketData> SendInnerFunc = null!;
    Action<ServerPacketData> MatchInnerFunc = null!;

    OmokGame _game;

    DateTime _roomMatchingTime;
    DateTime _gameStartTime;

    TimeSpan _maxGameTime;
    TimeSpan _maxMatchingWaitingTime;

    public Room(Int32 roomId)
    {
        RoomID = roomId;
        _game = new OmokGame();
        State = RoomState.Empty;
    }

    public void SetGameMatching(string firstUserID, string secondUserID)
    {
        State = RoomState.Mathcing;
        _matchingUsers.Add(firstUserID);
        _matchingUsers.Add(secondUserID);
        _roomMatchingTime = DateTime.Now;
    }

    public void InitDefaultSetting(Int32 turnTimeoutSecond, Int32 timeoutCount
                                    , Int32 maxGameTimeMinute, Int32 maxMatchingWaitingTimeSecond)
    {
        _game.TimeoutCount = timeoutCount;
        _game.TurnTimeoutSecond = turnTimeoutSecond;
        _maxGameTime = new TimeSpan(0, maxGameTimeMinute, 0);
        _maxMatchingWaitingTime = new TimeSpan(0, 0, maxMatchingWaitingTimeSecond);
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
        _game.InitLogger(logger);
    }

    public void SetDelegate(Func<string, byte[], bool> sendFunc, Func<string, User?> getUserInfoFunc
                            , Action<ServerPacketData> databaseSendFunc
                            , Action<ServerPacketData> sendInnerFunc
                            , Action<ServerPacketData> matchInnerFunc)
    {
        SendFunc = sendFunc;
        GetUserInfoFunc = getUserInfoFunc;
        SendInnerFunc = sendInnerFunc;
        MatchInnerFunc = matchInnerFunc;

        _game.SendFunc = SendFunc;
        _game.DatabaseSendFunc = databaseSendFunc;
        _game.GetUserInfoFunc = GetUserInfoFunc;

        _game.RoomClearFunc = RoomClear;
    }

    public void EnterRoom(User user)
    {
        if (State != RoomState.Mathcing)
        {
            SendFailedResponse<SRoomEnterRes>(user.SessionID, ErrorCode.NOT_MATCHING_ROOM, PacketType.RES_S_ROOM_ENTER);
            return;
        }

        if (_matchingUsers.Contains(user.UserID) == false)
        {
            SendFailedResponse<SRoomEnterRes>(user.SessionID, ErrorCode.NOT_MATCHING_USER, PacketType.RES_S_ROOM_ENTER);
            return;
        }

        RoomUser roomUser = new RoomUser();
        roomUser.SessionID = user.SessionID;
        roomUser.UserID = user.UserID;

        try
        {
            _users.Add(roomUser);

            {
                var req = new SNewUserEnterReq();
                req.User = user.Data;

                var bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_NEW_USER_ENTER);
                BroadCast(bytes, user.SessionID);
            }

            {
                var userList = new List<UserData>();
                foreach (var u in _users)
                {
                    var userInfo = GetUserInfoFunc(u.SessionID);
                    if (userInfo == null)
                    {
                        continue;
                    }

                    userList.Add(userInfo.Data);
                }

                var res = new SRoomEnterRes();
                res.ErrorCode = ErrorCode.NONE;
                res.UserList = userList;

                var bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_ENTER);
                SendFunc(user.SessionID, bytes);
            }

            user.RoomID = RoomID;
        }
        catch (Exception e)
        {
            Logger.Error($"EnterRoom : {e.Message}");
            SendFailedResponse<SRoomEnterRes>(user.SessionID, ErrorCode.EXCEPTION_ADD_USER, PacketType.RES_S_ROOM_ENTER);
        }
    }

    public void LeaveRoom(string sessionID)
    {
        // 매칭 후 성공하여 방으로 들어왔지만, 레디를 하지 않고 나가는 상황에 따른 처리로 변경해야됨.
        var user = GetRoomUser(sessionID);
        if (user == null)
        {
            SendFailedResponse<SRoomLeaveRes>(sessionID, ErrorCode.NOT_EXIST_ROOM_LEAVE_USER_DATA, PacketType.RES_S_ROOM_LEAVE);
            return;
        }

        if (_game.IsStart)
        {
            _game.LeaveGameEnd(sessionID);
            return;
        }

        _users.Remove(user);

        var userInfo = GetUserInfoFunc(sessionID)!;
        userInfo.LeaveRoom();

        {
            var res = new SRoomLeaveRes();
            res.ErrorCode = ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_LEAVE);
            SendFunc(sessionID, bytes);
        }

        {
            var req = new SUserLeaveReq();
            req.UserID = user.UserID;

            byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_USER_LEAVE);
            BroadCast(bytes, sessionID);
        }

        if (_users.Count == 0)
        {
            RoomClear();
        }

        DisconnectRoomUser(user);
    }

    public void SendChat(string sessionID, string message)
    {
        var user = GetRoomUser(sessionID);
        if (user == null)
        {
            SendFailedResponse<SRoomChatRes>(sessionID, ErrorCode.NOT_EXIST_ROOM_CHAT_USER_DATA, PacketType.RES_S_ROOM_CHAT);
            return;
        }

        var userInfo = GetUserInfoFunc(user.SessionID);
        if (userInfo == null)
        {
            SendFailedResponse<SRoomChatRes>(sessionID, ErrorCode.NOT_EXIST_USER, PacketType.RES_S_ROOM_CHAT);
            return;
        }

        var res = new SRoomChatRes();
        res.ErrorCode = ErrorCode.NONE;
        res.UserName = userInfo.NickName;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_CHAT);
        BroadCast(bytes);
    }

    public void GameReady(string sessionID, bool isReady)
    {
        if (_game.IsStart)
        {
            SendFailedResponse<SGameReadyRes>(sessionID, ErrorCode.ALREADY_START_GAME, PacketType.RES_S_GAME_READY);
            return;
        }

        var user = GetRoomUser(sessionID);
        if (user == null)
        {
            SendFailedResponse<SGameReadyRes>(sessionID, ErrorCode.NOT_EXIST_ROOM_READY_USER_DATA, PacketType.RES_S_GAME_READY);
            return;
        }

        user.IsReady = isReady;

        var res = new SGameReadyRes();
        res.ErrorCode = ErrorCode.NONE;
        res.IsReady = isReady;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_GAME_READY);
        BroadCast(bytes);

        if (_users.Count < 2)
        {
            return;
        }

        bool isAllReady = true;
        foreach (var u in _users)
        {
            if (u.IsReady == false)
            {
                isAllReady = false;
                break;
            }
        }

        if (isAllReady)
        {
            GameStart();
        }
    }

    public void GamePut(string sessionID, Int32 x, Int32 y)
    {
        if (_game.IsStart == false)
        {
            SendFailedResponse<SGamePutRes>(sessionID, ErrorCode.NOT_START_GAME, PacketType.RES_S_GAME_PUT);
            return;
        }

        _game.GamePut(sessionID, x, y);
    }

    public RoomUser? GetRoomUser(string sessionID)
    {
        return _users.Find(u => u.SessionID == sessionID);
    }

    void GameStart()
    {
        var random = new Random();
        Int32 startRand = random.Next(1000);
        Int32 currentPlayer = startRand % 2;

        _users[currentPlayer].PlayerColor = BoardType.Black;
        _users[(currentPlayer + 1) % 2].PlayerColor = BoardType.White;

        {
            var req = new SGameStartReq();
            req.StartPlayerID = _users[currentPlayer].UserID;
            req.IsStart = true;

            byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_START);
            BroadCast(bytes);
        }

        _game.GameStart(_users, currentPlayer);

        _gameStartTime = DateTime.Now;
    }

    void BroadCast(byte[] bytes, string expiredSessionID = "")
    {
        foreach (var user in _users)
        {
            if (user.SessionID == expiredSessionID)
            {
                continue;
            }

            SendFunc(user.SessionID, bytes);
        }
    }

    void DisconnectRoomUser(RoomUser user)
    {
        NTFUserDisconnectedReq ntf = new NTFUserDisconnectedReq();
        ntf.SessionID = user.SessionID;

        ServerPacketData packet = PacketManager.MakeInnerPacket(user.SessionID, ntf, InnerPacketType.NTF_USER_DISCONNECTED);
        SendInnerFunc(packet);
    }

    void RoomClear()
    {
        foreach (var user in _users)
        {
            var userInfo = GetUserInfoFunc(user.SessionID);
            if (userInfo == null)
            {
                continue;
            }

            userInfo.LeaveRoom();
            DisconnectRoomUser(user);
        }

        SetEmptyRoom();

        _users.Clear();
        _matchingUsers.Clear();
        _game.GameClear();
    }

    void SetEmptyRoom()
    {
        State = RoomState.Empty;
        
        MakeEmptyRoomReq req = new MakeEmptyRoomReq();
        req.RoomID = RoomID;

        var packet = MatchManager.MakeInnerPacket(MatchInnerType.MAKE_EMPTY_ROOM, req);
        MatchInnerFunc(packet);
    }

    void SendFailedResponse<T>(string sessionID, ErrorCode errorCode, PacketType packetType) where T : IResMessage, new()
    {
        Logger.Error($"Failed Room Action : {errorCode}");

        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, packetType);
        SendFunc(sessionID, bytes);
    }

    public void RoomCheck()
    {
        if (_game.IsStart)
        {
            if (_users.Count < 2)
            {
                _game.GameCancle();
            }

            TimeSpan ts = DateTime.Now - _gameStartTime;
            if (ts > _maxGameTime)
            {
                _game.GameCancle();
            }

            _game.TurnTimeoutCheck();
        }

        if (State == RoomState.Mathcing)
        {
            if (_users.Count == 2)
            {
                return;
            }

            TimeSpan ts = DateTime.Now - _roomMatchingTime;
            if (ts > _maxMatchingWaitingTime)
            {
                RoomClear();
            }
        }
    }
}