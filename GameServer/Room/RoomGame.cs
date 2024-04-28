using Common;

namespace GameServer;

// 게임 관련 기능
public partial class Room
{
    public const int BoardSize = 19;
    BoardType[,] _gameBoard;

    public void GameReady(string sessionID, bool isReady)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            return;
        }

        user.IsReady = isReady;

        SGameReadyRes res = new SGameReadyRes();
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

    public void GameStart()
    {
        // 유저에게 게임 시작을 알리고
        // 색깔 배정
        // 턴을 정하고
        // 게임 시작을 알린다.
        Random random = new Random();
        int startRand = random.Next(1000);
        CurrentPlayer = startRand % 2;

        _users[CurrentPlayer].PlayerColor = BoardType.Black;
        _users[(CurrentPlayer + 1) % 2].PlayerColor = BoardType.White;

        SGameStartReq req = new SGameStartReq();
        req.StartPlayerID = _users[CurrentPlayer].UserID;
        req.IsStart = true;
        req.RoomNumber = RoomID;
        
        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_START);
        BroadCast(bytes);

        IsStart = true;
    }

    public void GameCancle()
    {
        SGameCancleReq req = new SGameCancleReq();
        req.IsCancle = true;
        req.RoomID = RoomID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_CANCLE);
        BroadCast(bytes);

        Clear();
    }

    public void GameEnd(string winerSessionID)
    {
        var user = _users.Find(u => u.SessionID == winerSessionID);
        if (user == null)
        {
            return;
        }

        SGameEndReq req = new SGameEndReq();
        req.WinUserID = user.UserID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_END);
        BroadCast(bytes);

        Clear();
    }

    public void TurnChange()
    {
        CurrentPlayer = (CurrentPlayer + 1) % 2;

        STurnChangeReq req = new STurnChangeReq();
        req.NextTurnUserID = _users[CurrentPlayer].UserID;
        req.RoomNumber = RoomID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_TURN_CHANGE);
        BroadCast(bytes);
    }

    public void GamePut(string sessionID, int x, int y)
    {
        var user = _users.Find(u => u.SessionID == sessionID);
        if (user == null)
        {
            return;
        }

        if (_gameBoard[x, y] != BoardType.None)
        {
            return;
        }

        _gameBoard[x, y] = user.PlayerColor;
        if (CheckWin(x, y, user.PlayerColor))
        {
            GameEnd(sessionID);
        }
        else
        {
            TurnChange();
        }
    }

    bool CheckWin(int row, int col, BoardType currentPlayer)
    {
        // 가로 체크
        for (int c = 0; c <= BoardSize - 5; c++)
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
        for (int r = 0; r <= BoardSize - 5; r++)
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
        for (int r = 0; r <= BoardSize - 5; r++)
        {
            for (int c = 0; c <= BoardSize - 5; c++)
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
        for (int r = 0; r <= BoardSize - 5; r++)
        {
            for (int c = BoardSize - 1; c >= 4; c--)
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
}