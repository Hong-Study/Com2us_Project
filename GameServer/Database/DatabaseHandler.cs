using Common;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseHandler
{
    public Func<string, Task<UserRepository.GetUserGameDataResult>> GetUserGameDataAsync { get; set; } = null!;
    public Func<string, Int32, Int32, Task<ErrorCode>> UpdateUserWinLoseAsync { get; set; } = null!;

    public Action<ServerPacketData> InnerSendFunc = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public async Task Handle_DB_Login(string sessionID, IMessage message)
    {
        var packet = message as DBUserLoginReq;
        if (packet == null)
        {
            return;
        }

        var data = await GetUserGameDataAsync(packet.UserID);

        var res = new NTFUserLoginRes();
        res.ErrorCode = data.errorCode;
        res.UserData = data.userData;

        var serverPacketData = PacketManager.MakeInnerPacket(sessionID, res, InnerPacketType.NTF_RES_USER_LOGIN);
        InnerSendFunc(serverPacketData);
    }

    // 비동기로 이미 작동하기 때문에 그냥 동기로 만들어도 된다.
    // 누가 봐도 이게 더 이해하기 쉬울 것 같다. 는 코드를 작성해라.
    public async Task Handle_DB_UpdateWinLoseCount(string sessionID, IMessage message)
    {
        var packet = message as DBUpdateWinLoseCountReq;
        if (packet == null)
        {
            return;
        }

        var errorCode = await UpdateUserWinLoseAsync(packet.UserID, packet.WinCount, packet.LoseCount);

        var res = new NTFUserWinLoseUpdateRes();
        res.ErrorCode = errorCode;

        var serverPacketData = PacketManager.MakeInnerPacket(sessionID, res, InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT);
        InnerSendFunc(serverPacketData);
    }
}