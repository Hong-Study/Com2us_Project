namespace GameServer;

// 게임 관련 기능
public partial class Room : JobQueue
{

    public void GameReady(string SessionID, bool IsReady)
    {
        if (_users.TryGetValue(SessionID, out RoomUser? user))
        {
            user.IsReady = IsReady;
        }

        bool isAllReady = true;
        foreach (var roomUser in _users.Values)
        {
            if (roomUser.IsReady == false)
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

    }

    public void GameCancle()
    {

    }

    public void GameEnd()
    {

    }
    
    public void GamePut(string SessionID, int X, int Y)
    {

    }
}