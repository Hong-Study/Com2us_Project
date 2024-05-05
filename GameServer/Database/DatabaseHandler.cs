using Common;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseHandler
{
    public Func<Int64, Task<UserRepository.GetUserGameDataResult>> GetUserGameDataAsync { get; set; } = null!;
    public Func<Int64, Int32, Int32, Task<ErrorCode>> UpdateUserWinLoseAsync { get; set; } = null!;

    public Action<ServerPacketData> InnerSendFunc = null!;

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