using Common;

namespace GameServer;

public interface IUserRepository
{
    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger);
    public Task<UserRepository.GetUserGameDataResult> GetUserGameDataAsync(string userID);
    public Task<ErrorCode> UpdateUserWinLoseAsync(string userID, Int32 winCount, Int32 loseCount);
}