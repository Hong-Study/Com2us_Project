using Common;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseHandler
{
    public Func<DBConnector, string, DBRepository.GetUserGameDataResult> GetUserGameDataAsync { get; set; } = null!;
    public Func<DBConnector, string, Int32, Int32, ErrorCode> UpdateUserWinLoseAsync { get; set; } = null!;

    public Action<ServerPacketData> InnerSendFunc = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public void Handle_DB_Login(string sessionID, IMessage message, DBConnector connector)
    {
        var packet = message as DBUserLoginReq;
        if (packet == null)
        {
            return;
        }

        var data = GetUserGameDataAsync(connector, packet.UserID);

        var res = new NTFUserLoginRes();
        res.ErrorCode = data.errorCode;
        res.UserData = data.userData;

        var serverPacketData = PacketManager.MakeInnerPacket(sessionID, res, InnerPacketType.NTF_RES_USER_LOGIN);
        InnerSendFunc(serverPacketData);
    }

    public void Handle_DB_UpdateWinLoseCount(string sessionID, IMessage message, DBConnector connector)
    {
        var packet = message as DBUpdateWinLoseCountReq;
        if (packet == null)
        {
            return;
        }

        var errorCode = UpdateUserWinLoseAsync(connector, packet.UserID, packet.WinCount, packet.LoseCount);

        var res = new NTFUserWinLoseUpdateRes();
        res.ErrorCode = errorCode;

        var serverPacketData = PacketManager.MakeInnerPacket(sessionID, res, InnerPacketType.NTF_RES_UPDATE_WIN_LOSE_COUNT);
        InnerSendFunc(serverPacketData);
    }
}