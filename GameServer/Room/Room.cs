using Common;

namespace GameServer;

// 방 관련 기능
public class Room : JobQueue
{
    List<RoomUser> _users = new List<RoomUser>();
    Func<string, byte[], bool> SendFunc = null!;

    public Int32 RoomID { get; private set; }
    public bool IsStart { get; private set; } = false;

    OmokGame _game;

    public Room(Int32 roomId)
    {
        RoomID = roomId;
        _game = new OmokGame(SendFunc);
    }

    public void Init(Func<string, byte[], bool> SendFunc)
    {
        this.SendFunc = SendFunc;
    }

    public void EnterRoom(User user)
    {
        if (_users.Count >= 2)
        {
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

            // 기존의 유저들에게 새로운 유저 정보를
            {
                var req = new SNewUserEnterReq();
                req.User = roomUser.UserData;

                var bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_NEW_USER_ENTER);
                BroadCast(bytes, user.SessionID);
            }

            // 나에게 들어온 유저의 정보를
            {
                var userList = new List<UserData>();
                foreach (var u in _users)
                {
                    userList.Add(u.UserData);
                }

                var res = new SRoomEnterRes();
                res.ErrorCode = (Int16)ErrorCode.NONE;
                res.UserList = userList;

                var bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_ENTER);
                SendFunc(user.SessionID, bytes);
            }

            user.RoomID = RoomID;
        }
        catch (Exception e)
        {
            MainServer.MainLogger.Error($"EnterRoom : {e.Message}");
        }
    }

    public void LeaveRoom(string sessionID)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            return;
        }

        _users.Remove(user);

        // 나간 유저에게 나간 것을 알림
        {
            var res = new SRoomLeaveRes();
            res.ErrorCode = (Int16)ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_LEAVE);
            SendFunc(sessionID, bytes);
        }

        // 남은 유저에게 나간 유저를 알림
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
            return;
        }

        var res = new SRoomChatRes();
        res.ErrorCode = ErrorCode.NONE;
        res.UserName = user.SessionID;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_CHAT);
        BroadCast(bytes);
    }

    public void GameReady(string sessionID, bool isReady)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            return;
        }

        user.IsReady = isReady;

        var res = new SGameReadyRes();
        res.ErrorCode = (Int16)ErrorCode.NONE;
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
        _game.GamePut(sessionID, x, y);
    }

    public void GameEnd()
    {

    }

    public void GameCancle()
    {
        
    }

    void GameStart()
    {
        var random = new Random();
        int startRand = random.Next(1000);
        int currentPlayer = startRand % 2;

        _users[currentPlayer].PlayerColor = BoardType.Black;
        _users[(currentPlayer + 1) % 2].PlayerColor = BoardType.White;

        var req = new SGameStartReq();
        req.StartPlayerID = _users[currentPlayer].UserID;
        req.IsStart = true;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_START);
        BroadCast(bytes);
        
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
}
