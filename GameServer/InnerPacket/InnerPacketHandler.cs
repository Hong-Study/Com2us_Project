using Common;

namespace GameServer;

public partial class PacketHandler
{
    public void HandleNTFCheckSessionLogin(string sessionID, IMessage message)
    {
        NTFCheckSessionLoginReq? packet = message as NTFCheckSessionLoginReq;
        if (packet == null)
        {
            return;
        }

        SessionLoginTimeoutCheckFunc();
    }

    public void HandleNTFHeartBeat(string sessionID, IMessage message)
    {
        NTFHeartBeatReq? packet = message as NTFHeartBeatReq;
        if (packet == null)
        {
            return;
        }

        HeartHeatCheckFunc();
    }

    public void HandleNTFRoomsCheck(string sessionID, IMessage message)
    {
        NTFRoomsCheckReq? packet = message as NTFRoomsCheckReq;
        if (packet == null)
        {
            return;
        }

        RoomCheckFunc();
    }

    public void HandleNTFSessionConnected(string sessionID, IMessage message)
    {
        NTFSessionConnectedReq? packet = message as NTFSessionConnectedReq;
        if (packet == null)
        {
            return;
        }

        AddUserFunc(packet.SessionID);
    }

    public void HandleNTFSessionDisconnected(string sessionID, IMessage message)
    {
        NTFSessionDisconnectedReq? packet = message as NTFSessionDisconnectedReq;
        if (packet == null)
        {
            return;
        }

        var user = GetUserFunc(packet.SessionID);
        if (user != null)
        {
            var room = GetRoombyIDFunc(user.RoomID);
            if (room != null)
            {
                room.LeaveRoom(sessionID);
            }
        }

        RemoveUserFunc(packet.SessionID);
    }

    public void HandleNTFUserLogin(string sessionID, IMessage message)
    {
        NTFUserLoginRes? packet = message as NTFUserLoginRes;
        if (packet == null)
        {
            return;
        }

        LoginUserFunc(sessionID, packet.ErrorCode, packet.UserData);
    }

    public void HandleNTFUpdateWinLoseCount(string sessionID, IMessage message)
    {
        NTFUserWinLoseUpdateRes? packet = message as NTFUserWinLoseUpdateRes;
        if (packet == null)
        {
            return;
        }
    }

    public void HandleNTFMatchingRoom(string sessionID, IMessage message)
    {
        NTFMatchingReq? packet = message as NTFMatchingReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoombyNumberFunc(packet.RoomNumber);
        if (room == null)
        {
            return;
        }

        room.SetGameMatching(packet.FirstUserID, packet.SecondUserID);
    }

    public void HandleNTFUserDisconnected(string sessionID, IMessage message)
    {
        var session = GetSessionFunc(sessionID);
        if(session == null)
        {
            return;
        }

        session.Close();
    }
}