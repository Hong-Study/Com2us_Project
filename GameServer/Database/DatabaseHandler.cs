using Common;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseHandler
{
    public Func<Int64, Task<UserGameData?>> GetUserGameDataAsync { get; set; } = null!;
    public Func<Int64, Int32, Int32, Task<bool>> UpdateUserWinLoseAsync { get; set; } = null!;

    public async Task Handle_DB_Login(string sessionID, IMessage message)
    {
        var packet = message as DBUserLoginReq;
        if (packet == null)
        {
            return;
        }

        var data = await GetUserGameDataAsync(packet.UserID);
        if (data == null)
        {
            return;
        }


    }

    public async Task Handle_DB_UpdateWinLoseCount(string sessionID, IMessage message)
    {
        var packet = message as DBUpdateWinLoseCountReq;
        if (packet == null)
        {
            return;
        }

        await Task.CompletedTask;
    }
}