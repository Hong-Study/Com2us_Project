using Common;

namespace GameServer;

// 방 관련 기능
public class Room
{
    List<RoomUser> _users = new List<RoomUser>();
    Func<string, byte[], bool> SendFunc = null!;

    public Int32 RoomID { get; private set; }
    public bool IsStart { get; private set; } = false;

    public OmokGame _game;

    public Room(Int32 roomId)
    {
        RoomID = roomId;
        _game = new OmokGame();
    }

    public void SetDelegate(Func<string, byte[], bool> SendFunc)
    {
        this.SendFunc = SendFunc;

        _game.SetDelegate(SendFunc, GameEnd);
    }

    public void EnterRoom(User user)
    {
        if (_users.Count >= 2)
        {
            SendFailedResponse<SRoomEnterRes>(user.SessionID, ErrorCode.FULL_ROOM_COUNT, PacketType.RES_S_ROOM_ENTER);
            return;
        }

        RoomUser roomUser = new RoomUser();
        roomUser.SessionID = user.SessionID;
        roomUser.UserID = user.UserID;
        roomUser.UserData = new UserData()
        {
            UserID = user.UserID,
            NickName = user.NickName,
            Level = user.Level,
            Win = user.Win,
            Lose = user.Lose,
            PlayerColor = (Int16)BoardType.None,
        };

        try
        {
            _users.Add(roomUser);
            MainServer.MainLogger.Debug($"EnterRoom : {user.UserID} : {_users.Count}");

            {
                var req = new SNewUserEnterReq();
                req.User = roomUser.UserData;

                var bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_NEW_USER_ENTER);
                BroadCast(bytes, user.SessionID);
            }

            {
                var userList = new List<UserData>();
                foreach (var u in _users)
                {
                    userList.Add(u.UserData);
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
            MainServer.MainLogger.Error($"EnterRoom : {e.Message}");
            SendFailedResponse<SRoomEnterRes>(user.SessionID, ErrorCode.ADD_USER_EXCEPTION, PacketType.RES_S_ROOM_ENTER);
        }
    }

    public void LeaveRoom(string sessionID)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            SendFailedResponse<SRoomLeaveRes>(sessionID, ErrorCode.NOT_EXIST_ROOM_LEAVE_USER_DATA, PacketType.RES_S_ROOM_LEAVE);
            return;
        }

        _users.Remove(user);

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
    }

    public void SendChat(string sessionID, string message)
    {
        // 가져오는 코드
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            SendFailedResponse<SRoomChatRes>(sessionID, ErrorCode.NOT_EXIST_ROOM_CHAT_USER_DATA, PacketType.RES_S_ROOM_CHAT);
            return;
        }

        var res = new SRoomChatRes();
        res.ErrorCode = ErrorCode.NONE;
        res.UserName = user.UserData.NickName;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_CHAT);
        BroadCast(bytes);
    }

    public void GameReady(string sessionID, bool isReady)
    {
        if (IsStart)
        {
            SendFailedResponse<SGameReadyRes>(sessionID, ErrorCode.ALREADY_START_GAME, PacketType.RES_S_GAME_READY);
            return;
        }

        var user = _users.Find(u => u.SessionID == sessionID);
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

    public void GamePut(string sessionID, int x, int y)
    {
        if (IsStart == false)
        {
            SendFailedResponse<SGamePutRes>(sessionID, ErrorCode.NOT_START_GAME, PacketType.RES_S_GAME_PUT);
            return;
        }

        _game.GamePut(sessionID, x, y);
    }

    public void GameEnd(string winnerSessionID)
    {
        IsStart = false;
        _game.GameClear();
    }

    void GameStart()
    {
        var random = new Random();
        int startRand = random.Next(1000);
        int currentPlayer = startRand % 2;

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

        IsStart = true;
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

    void Clear()
    {
        _users.Clear();
        IsStart = false;

        // 복사로 초기화해버리기
    }

    void SendFailedResponse<T>(string sessionID, ErrorCode errorCode, PacketType packetType) where T : IResMessage, new()
    {
        MainServer.MainLogger.Error($"Failed Room Action : {errorCode}");

        var res = new T();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, packetType);
        SendFunc(sessionID, bytes);
    }
}
