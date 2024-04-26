using System.Collections.Concurrent;

namespace GameServer;

// 방 관련 기능
public partial class Room : JobQueue
{
    List<RoomUser> _users = new List<RoomUser>();

    Func<string, byte[], bool> SendFunc = null!;

    public Int32 RoomID { get; private set; }
    public bool IsStart { get; private set; } = false;
    public int CurrentPlayer { get; private set; } = 0;

    public Room(Int32 roomId)
    {
        _gameBoard = new BoardType[BoardSize, BoardSize];
        RoomID = roomId;
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

            // 기존의 유저들에게 새로운 유저 정보를
            {
                SNewUserEnterReq req = new SNewUserEnterReq();
                req.User = roomUser.UserData;

                byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_NEW_USER_ENTER);
                BroadCast(bytes, user.SessionID);
            }
            // 나에게 들어온 유저의 정보를
            {
                var userList = new List<UserData>();
                foreach (var u in _users)
                {
                    userList.Add(u.UserData);
                }

                SRoomEnterRes res = new SRoomEnterRes();
                res.ErrorCode = (Int16)ErrorCode.NONE;
                res.UserList = userList;

                byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_ENTER);
                SendFunc(user.SessionID, bytes);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
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
            SRoomLeaveRes res = new SRoomLeaveRes();
            res.ErrorCode = (Int16)ErrorCode.NONE;

            byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_LEAVE);
            SendFunc(sessionID, bytes);
        }

        // 남은 유저에게 나간 유저를 알림
        {
            SUserLeaveReq req = new SUserLeaveReq();
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

        SRoomChatRes res = new SRoomChatRes();
        res.UserName = user.SessionID;
        res.Message = message;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_ROOM_CHAT);
        BroadCast(bytes);
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

        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                _gameBoard[i, j] = BoardType.None;
            }
        }
    }
}

enum BoardType : Int16
{
    None = 0,
    Black = 1,
    White = 2,
}

class RoomUser
{
    public string SessionID { get; set; } = null!;
    public Int64 UserID { get; set; }
    public bool IsReady { get; set; } = false;
    public BoardType PlayerColor { get; set; }
    public UserData UserData { get; set; } = null!;
}