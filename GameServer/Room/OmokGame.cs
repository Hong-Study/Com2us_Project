using Common;

namespace GameServer;

public class OmokGame
{
    public const Int32 BoardSize = 19;

    BoardType[,] _gameBoard;

    Func<string, byte[], bool> SendFunc = null!;

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

    public void SetDelegate(Func<string, byte[], bool> sendFunc)
    {
        SendFunc = sendFunc;
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
    }

    public void GameEnd(bool isNextUserWin = false)
    {
        if (isNextUserWin)
        {
            CurrentPlayer = GetNextTurn();
        }

        var user = _users[CurrentPlayer];

        SGameEndReq req = new SGameEndReq();
        req.WinUserID = user.UserID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_END);
        BroadCast(bytes);
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
        Array.Clear(_gameBoard, 0, _gameBoard.Length);

        IsStart = false;
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
            GameEnd(true);

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
        MainServer.MainLogger.Error($"Failed OmokGame Action : {errorCode}");

        var res = new I();
        res.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(res, packetType);
        SendFunc(sessionID, bytes);
    }


}