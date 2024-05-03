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

        HeartHeatCheckFunc();
    }

    public void Handle_NTFRoomsCheck(string sessionID, IMessage message)
    {
        NTFRoomsCheckReq? packet = message as NTFRoomsCheckReq;
        if (packet == null)
        {
            return;
        }

        RoomCheckFunc();
    }

    public void Handle_NTFSessionConnected(string sessionID, IMessage message)
    {
        NTFSessionConnectedReq? packet = message as NTFSessionConnectedReq;
        if (packet == null)
        {
            return;
        }

        AddUserFunc(packet.SessionID);
    }

    public void Handle_NTFSessionDisconnected(string sessionID, IMessage message)
    {
        NTFSessionDisconnectedReq? packet = message as NTFSessionDisconnectedReq;
        if (packet == null)
        {
            return;
        }

        var user = GetUserFunc(packet.SessionID);
        if (user != null)
        {
            var room = GetRoomFunc(user.RoomID);
            if (room != null)
            {
                room.LeaveRoom(sessionID);
            }
        }
        
        RemoveUserFunc(packet.SessionID);
    }
}