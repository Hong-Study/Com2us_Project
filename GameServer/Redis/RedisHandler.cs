using Common;

namespace GameServer;

public class RedisHandler
{
    public Func<string, string, RedisConnector, ErrorCode> ValidataeTokenFunc = null!;

    public Action<ServerPacketData> InnerSendFunc = null!;
    public Action<ServerPacketData> DatabaseSendFunc = null!;

    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }
    
    public void Handle_RD_UserLogin(string sessionID, IMessage message, RedisConnector connector)
    {
        var packet = message as RDUserLoginReq;
        if (packet == null)
        {
            return;
        }

        var errorCode = ValidataeTokenFunc(packet.UserID, packet.AuthToken, connector);

        if(errorCode != ErrorCode.NONE)
        {
            var res = new NTFUserLoginRes();
            res.ErrorCode = errorCode;

            var serverPacketData = PacketManager.MakeInnerPacket(sessionID, res, InnerPacketType.NTF_RES_USER_LOGIN);
            InnerSendFunc(serverPacketData);
        }
        else
        {
            var req = new DBUserLoginReq();

            req.UserID = packet.UserID;
            
            var serverPacketData = DatabaseManager.MakeDatabasePacket(sessionID, req, DatabaseType.REQ_DB_USER_LOGIN);
            DatabaseSendFunc(serverPacketData);
        }
    }
}