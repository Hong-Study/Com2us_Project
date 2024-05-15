using Common;
using SqlKata;

namespace GameServer;

public partial class PacketHandler
{
    public void Handle_C_RoomEnter(string sessionID, IMessage message)
    {
        CRoomEnterReq? packet = message as CRoomEnterReq;
        if (packet == null)
        {
            return;
        }

        var user = GetUserFunc(sessionID);
        if (user == null)
        {
            Logger.Error($"GetUser : User{sessionID} is not exist");

            SendFailEnterRoomRes(sessionID, ErrorCode.NOT_EXIST_USER);

            return;
        }

        if (user.RoomID != 0)
        {
            Logger.Error($"GetUser : User{sessionID} is already in room");

            SendFailEnterRoomRes(sessionID, ErrorCode.ALREADY_IN_ROOM);

            return;
        }

        var room = GetRoomFunc(packet.RoomNumber);
        if (room == null)
        {
            Logger.Error($"GetRoom : Room({user.UserID}) is not exist");

            SendFailEnterRoomRes(sessionID, ErrorCode.NOT_EXIST_ROOM);

            return;
        }

        room.EnterRoom(user);
    }

    public void Handle_C_RoomLeave(string sessionID, IMessage message)
    {
        CRoomLeaveReq? packet = message as CRoomLeaveReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoom<SRoomLeaveRes>(sessionID, PacketType.RES_S_ROOM_LEAVE);
        if (room != null)
        {
            room.LeaveRoom(sessionID);
        }
    }

    public void Handle_C_RoomChat(string sessionID, IMessage message)
    {
        CRoomChatReq? packet = message as CRoomChatReq;
        if (packet == null)
        {
            return;
        }

        Logger.Debug($"Room Chat : {packet.Message}");

        Room? room = GetRoom<SRoomChatRes>(sessionID, PacketType.RES_S_ROOM_CHAT);
        if (room != null)
        {
            room.SendChat(sessionID, packet.Message);
        }
    }

    void SendFailEnterRoomRes(string sessionID, ErrorCode errorCode)
    {
        SRoomEnterRes pkt = new SRoomEnterRes();
        pkt.ErrorCode = errorCode;

        byte[] bytes = PacketManager.PacketSerialized(pkt, PacketType.RES_S_ROOM_ENTER);
        SendFunc(sessionID, bytes);

        var session = GetSessionFunc(sessionID);
        if (session != null)
        {
            session.Close();
        }
    }
}