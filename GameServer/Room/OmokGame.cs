using Common;
using SqlKata;

namespace GameServer;

// 게임 관련 기능
public class OmokGame
{
    public const int BoardSize = 19;

    BoardType[,] _gameBoard;

    Func<string, byte[], bool> SendFunc;
    List<RoomUser> _users = null!;
    public int CurrentPlayer { get; private set; } = 0;

    public OmokGame(Func<string, byte[], bool> sendFunc)
    {
        _gameBoard = new BoardType[BoardSize, BoardSize];
        SendFunc = sendFunc;
    }

    public void GameStart(List<RoomUser> users, int currentPlayer)
    {
        // 유저에게 게임 시작을 알리고
        // 색깔 배정
        // 턴을 정하고
        // 게임 시작을 알린다.
        GameClear();

        CurrentPlayer = currentPlayer;
        _users = users;
    }

    public void GameCancle()
    {
        SGameCancleReq req = new SGameCancleReq();
        req.IsCancle = true;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_GAME_CANCLE);
        BroadCast(bytes);
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

    void TurnChange()
    {
        CurrentPlayer = (CurrentPlayer + 1) % 2;

        STurnChangeReq req = new STurnChangeReq();
        req.NextTurnUserID = _users[CurrentPlayer].UserID;

        byte[] bytes = PacketManager.PacketSerialized(req, PacketType.REQ_S_TURN_CHANGE);
        BroadCast(bytes);
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

    void GameClear()
    {
        Array.Clear(_gameBoard, 0, _gameBoard.Length);
    }
}