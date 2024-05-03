using Common;

namespace GameServer;

public class RedisHandler
{
    public async Task Handle_ME_UserLogin(string sessionID, IMessage message)
    {
        var packet = message as MEUserLoginReq;
        if (packet == null)
        {
            return;
        }

        await Task.CompletedTask;
    }
}