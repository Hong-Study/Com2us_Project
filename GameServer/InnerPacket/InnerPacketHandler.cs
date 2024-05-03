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

        SessionLoginTimeoutCheckFunc();
    }

    public void Handle_NTFHeartBeat(string sessionID, IMessage message)
    {
        NTFHeartBeatReq? packet = message as NTFHeartBeatReq;
        if (packet == null)
        {
            return;
        }

        // HeartBeat 처리
        HeartHeatCheckFunc();
    }

    public void Handle_NTFRoomsCheck(string sessionID, IMessage message)
    {
        NTFRoomsCheckReq? packet = message as NTFRoomsCheckReq;
        if (packet == null)
        {
            return;
        }

        // RoomCheck 처리
        RoomCheckFunc();
    }
}