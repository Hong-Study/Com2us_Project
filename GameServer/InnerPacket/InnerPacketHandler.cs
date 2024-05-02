using Common;

namespace GameServer;

public partial class PacketHandler
{
    public void Handle_NTFCheckSessionLogin(string sessionID, IMessage message)
    {
        NTFCheckSessionLoginReq? packet = message as NTFCheckSessionLoginReq;
        if (packet == null)
        {
            return;
        }

        SessionTimeoutCheckedFunc();
    }
}