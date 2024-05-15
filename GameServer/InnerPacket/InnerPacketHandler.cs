using Common;

namespace GameServer;

public partial class PacketHandler
{
    public void Handle_NTF_CheckSessionLogin(string sessionID, IMessage message)
    {
        NTFCheckSessionLoginReq? packet = message as NTFCheckSessionLoginReq;
        if (packet == null)
        {
            return;
        }

        SessionLoginTimeoutCheckFunc();
    }

    public void Handle_NTF_HeartBeat(string sessionID, IMessage message)
    {
        NTFHeartBeatReq? packet = message as NTFHeartBeatReq;
        if (packet == null)
        {
            return;
        }

        HeartHeatCheckFunc();
    }

    public void Handle_NTF_RoomsCheck(string sessionID, IMessage message)
    {
        NTFRoomsCheckReq? packet = message as NTFRoomsCheckReq;
        if (packet == null)
        {
            return;
        }

        RoomCheckFunc();
    }

    public void Handle_NTF_SessionConnected(string sessionID, IMessage message)
    {
        NTFSessionConnectedReq? packet = message as NTFSessionConnectedReq;
        if (packet == null)
        {
            return;
        }

        AddUserFunc(packet.SessionID);
    }

    public void Handle_NTF_SessionDisconnected(string sessionID, IMessage message)
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

    public void Handle_NTF_UserLogin(string sessionID, IMessage message)
    {
        NTFUserLoginRes? packet = message as NTFUserLoginRes;
        if (packet == null)
        {
            return;
        }

        LoginUserFunc(sessionID, packet.ErrorCode, packet.UserData);
    }

    public void Handle_NTF_UpdateWinLoseCount(string sessionID, IMessage message)
    {
        NTFUserWinLoseUpdateRes? packet = message as NTFUserWinLoseUpdateRes;
        if (packet == null)
        {
            return;
        }
    }

    public void Handle_NTF_MatchingRoom(string sessionID, IMessage message)
    {
        NTFMatchingReq? packet = message as NTFMatchingReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoomFunc(packet.RoomID);
        if (room == null)
        {
            return;
        }

        room.SetGameMatching(packet.FirstUserID, packet.SecondUserID);
    }

    public void Handle_NTF_UserDisconnected(string sessionID, IMessage message)
    {
        var session = GetSessionFunc(sessionID);
        if(session == null)
        {
            return;
        }

        session.Close();
    }
}