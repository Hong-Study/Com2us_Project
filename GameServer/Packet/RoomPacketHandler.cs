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
            MainServer.MainLogger.Error($"GetUser : User{sessionID} is not exist");
            
            SRoomEnterRes pkt = new SRoomEnterRes();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_USER;

            byte[] bytes = PacketManager.PacketSerialized(pkt, PacketType.RES_S_ROOM_ENTER);
            SendFunc(sessionID, bytes);

            return;
        }

        var room = GetRoomFunc(packet.RoomNumber);
        if (room == null)
        {
            MainServer.MainLogger.Error($"GetRoom : Room({user.UserID}) is not exist");

            SRoomEnterRes pkt = new SRoomEnterRes();
            pkt.ErrorCode = ErrorCode.NOT_EXIST_ROOM;

            byte[] bytes = PacketManager.PacketSerialized(pkt, PacketType.RES_S_ROOM_ENTER);
            SendFunc(sessionID, bytes);
            
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

        var room = GetRoom<SRoomLeaveRes>(sessionID);
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

        MainServer.MainLogger.Debug($"Room Chat : {packet.Message}");

        Room? room = GetRoom<SRoomChatRes>(sessionID);
        if (room != null)
        {
            room.SendChat(sessionID, packet.Message);
        }

    }
}