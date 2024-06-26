using Common;

namespace GameServer;

public class OmokGame
{
    public const Int32 BoardSize = 19;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    BoardType[,] _gameBoard;

    public Func<string, byte[], bool> SendFunc = null!;
    public Func<string, User?> GetUserInfoFunc = null!;
    public Action<ServerPacketData> DatabaseSendFunc = null!;

    public Action RoomClearFunc = null!;

    List<RoomUser> _users = null!;

    DateTime _turnStartTime;
    public Int32 TurnTimeoutSecond { get; set; } = 30;
    public Int32 TimeoutCount { get; set; } = 3;

    public Int32 CurrentPlayer { get; private set; } = 0;
    public bool IsStart { get; private set; } = false;

    public OmokGame()
    {
        _gameBoard = new BoardType[BoardSize, BoardSize];
    }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void SetDelegate(Func<string, byte[], bool> sendFunc, Action RoomClearFunc)
    {
        SendFunc = sendFunc;
        this.RoomClearFunc = RoomClearFunc;
    }

    public void GameStart(List<RoomUser> users, Int32 currentPlayer)
    {
        GameClear();

        _users = users;

        CurrentPlayer = currentPlayer;
        IsStart = true;

        _turnStartTime = DateTime.Now;
    }

    public void GameCancle()
    {
        SGameCancleReq req = new SGameCancleReq();
        req.IsCancle = true;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_CANCLE);
        BroadCast(bytes);

        RoomClearFunc();
    }

    public void TimeoutGameEnd()
    {
        CurrentPlayer = GetNextTurn();

        GameEnd();
    }

    public void LeaveGameEnd(string sessionID)
    {
        if (_users[CurrentPlayer].SessionID == sessionID)
        {
            CurrentPlayer = GetNextTurn();
        }

        GameEnd();
    }

    public void GameEnd()
    {
        var user = _users[CurrentPlayer];

        UpdateUserWinLoseCount(user.SessionID, true);
        UpdateUserWinLoseCount(_users[GetNextTurn()].SessionID, false);

        SGameEndReq req = new SGameEndReq();
        req.WinUserID = user.UserID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_END);
        BroadCast(bytes);

        RoomClearFunc();
    }

    public void GamePut(string sessionID, Int32 x, Int32 y)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            SendFailedResponse<SGamePutRes>(sessionID, ErrorCode.NOT_EXIST_USER, PacketType.RES_S_GAME_PUT);
            return;
        }

        if (_users[CurrentPlayer].UserID != user.UserID)
        {
            SendFailedResponse<SGamePutRes>(sessionID, ErrorCode.NOT_MY_TURN, PacketType.RES_S_GAME_PUT);
            return;
        }

        if (_gameBoard[x, y] != BoardType.None)
        {
            return;
        }

        user.TimeoutCount = 0;

        _gameBoard[x, y] = user.PlayerColor;

        var res = new SGamePutRes();
        res.PosX = x;
        res.PosY = y;
        res.ErrorCode = ErrorCode.NONE;
        res.UserID = user.UserID;

        byte[] bytes = PacketManager.PacketSerialized(res, PacketType.RES_S_GAME_PUT);
        BroadCast(bytes, sessionID);

        if (CheckWin(x, y, user.PlayerColor))
        {
            GameEnd();
        }
        else
        {
            TurnChange();
        }
    }

    public void TurnTimeoutCheck()
    {
        DateTime nowTime = DateTime.Now;
        TimeSpan turnTime = nowTime - _turnStartTime;

        if (turnTime.TotalSeconds > TurnTimeoutSecond)
        {
            TimeoutTurnChange();
        }
    }

    public void GameClear()
    {
        // _users.Clear();
        Array.Clear(_gameBoard, 0, _gameBoard.Length);

        IsStart = false;
    }

    public void UpdateUserWinLoseCount(string sessionID, bool isWin)
    {
        var user = GetUserInfoFunc(sessionID);
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

    void TurnChange()
    {
        CurrentPlayer = GetNextTurn();

        STurnChangeReq req = new STurnChangeReq();
        req.NextTurnUserID = _users[CurrentPlayer].UserID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_TURN_CHANGE);
        BroadCast(bytes);

        _turnStartTime = DateTime.Now;
    }

    void TimeoutTurnChange()
    {
        if (++_users[CurrentPlayer].TimeoutCount >= TimeoutCount)
        {
            TimeoutGameEnd();

            return;
        }

        TurnChange();
    }

    bool CheckWin(Int32 row, Int32 col, BoardType currentPlayer)
    {
        // 가로 체크
        for (Int32 c = 0; c <= BoardSize - 5; c++)
        {
            if (_gameBoard[row, c] == currentPlayer &&
                _gameBoard[row, c + 1] == currentPlayer &&
                _gameBoard[row, c + 2] == currentPlayer &&
                _gameBoard[row, c + 3] == currentPlayer &&
                _gameBoard[row, c + 4] == currentPlayer)
            {
                return true;
            }
        }

        // 세로 체크
        for (Int32 r = 0; r <= BoardSize - 5; r++)
        {
            if (_gameBoard[r, col] == currentPlayer &&
                _gameBoard[r + 1, col] == currentPlayer &&
                _gameBoard[r + 2, col] == currentPlayer &&
                _gameBoard[r + 3, col] == currentPlayer &&
                _gameBoard[r + 4, col] == currentPlayer)
            {
                return true;
            }
        }

        // 대각선 체크 (우상향)
        for (Int32 r = 0; r <= BoardSize - 5; r++)
        {
            for (Int32 c = 0; c <= BoardSize - 5; c++)
            {
                if (_gameBoard[r, c] == currentPlayer &&
                    _gameBoard[r + 1, c + 1] == currentPlayer &&
                    _gameBoard[r + 2, c + 2] == currentPlayer &&
                    _gameBoard[r + 3, c + 3] == currentPlayer &&
                    _gameBoard[r + 4, c + 4] == currentPlayer)
                {
                    return true;
                }
            }
        }

        // 대각선 체크 (우하향)
        for (Int32 r = 0; r <= BoardSize - 5; r++)
        {
            for (Int32 c = BoardSize - 1; c >= 4; c--)
            {
                if (_gameBoard[r, c] == currentPlayer &&
                    _gameBoard[r + 1, c - 1] == currentPlayer &&
                    _gameBoard[r + 2, c - 2] == currentPlayer &&
                    _gameBoard[r + 3, c - 3] == currentPlayer &&
                    _gameBoard[r + 4, c - 4] == currentPlayer)
                {
                    return true;
                }
            }
        }

        return false;
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

    Int32 GetNextTurn()
    {
        return (CurrentPlayer + 1) % 2;
    }

    void SendFailedResponse<I>(string sessionID, ErrorCode errorCode, PacketType packetType)
                            where I : IResMessage, new()
    {
        Logger.Error($"Failed OmokGame Action : {errorCode}");

        var res = new I();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, packetType);
        SendFunc(sessionID, bytes);
    }
}