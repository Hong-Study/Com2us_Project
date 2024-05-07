using Common;

namespace GameServer;

public interface IUserRepository
{
    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger);
    public Task<UserRepository.GetUserGameDataResult> GetUserGameDataAsync(Int64 userID);
    public Task<ErrorCode> UpdateUserWinLoseAsync(Int64 userID, Int32 winCount, Int32 loseCount);
}