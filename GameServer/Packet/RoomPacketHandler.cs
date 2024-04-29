using Common;

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

        // 방 입장 처리
        User? user = GetUserFunc(sessionID);
        if (user == null)
        {
            MainServer.MainLogger.Debug($"Room Enter : User is null");
            return;
        }

        MainServer.MainLogger.Debug($"Room Enter {user.UserID} : {packet.RoomNumber}");

        Room? room = GetRoomFunc(packet.RoomNumber);
        if (room == null)
        {
            MainServer.MainLogger.Debug($"Room Enter : Room is null");
            return;
        }

        room.Push(() => room.EnterRoom(user));
    }

    public void Handle_C_RoomLeave(string sessionID, IMessage message)
    {
        CRoomLeaveReq? packet = message as CRoomLeaveReq;
        if (packet == null)
        {
            return;
        }

        // 방 퇴장 처리
        User? user = GetUserFunc(sessionID);
        if (user == null)
        {
            return;
        }

        Room? room = GetRoomFunc(user.RoomID);
        if (room == null)
        {
            return;
        }

        room.Push(() => room.LeaveRoom(sessionID));
    }

    public void Handle_C_RoomChat(string sessionID, IMessage message)
    {
        CRoomChatReq? packet = message as CRoomChatReq;
        if (packet == null)
        {
            return;
        }

        MainServer.MainLogger.Debug($"Room Chat : {packet.Message}");

        // 방 채팅 처리
        User? user = GetUserFunc(sessionID);
        if (user == null)
        {
            MainServer.MainLogger.Debug($"Room Chat : User is null");
            return;
        }

        Room? room = GetRoomFunc(user.RoomID);
        if (room == null)
        {
            MainServer.MainLogger.Debug($"Room Chat : Room is null");
            return;
        }

        // 채팅 메시지 전파
        room.Push(() => room.SendChat(sessionID, packet.Message));
    }
}