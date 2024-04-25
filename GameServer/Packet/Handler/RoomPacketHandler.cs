namespace GameServer;

public partial class PacketHandler
{
    public Func<string, User?> GetUserFunc = null!;
    public Func<int, Room?> GetRoomFunc = null!;

    public void Handle_C_RoomEnter(string sessionID, IMessage message)
    {
        RoomEnterReq? packet = message as RoomEnterReq;
        if (packet == null)
        {
            return;
        }

        // 방 입장 처리
        User? user = GetUserFunc(sessionID);
        if (user == null)
        {
            return;
        }

        Room? room = GetRoomFunc(packet.RoomNumber);
        if (room == null)
        {
            return;
        }

        if (room.EnterRoom(user))
        {
            // 방 입장 성공
        }
    }

    public void Handle_C_RoomLeave(string sessionID, IMessage message)
    {
        RoomLeaveReq? packet = message as RoomLeaveReq;
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

        if (packet.RoomNumber != user.RoomID)
        {
            return;
        }

        Room? room = GetRoomFunc(user.RoomID);
        if (room == null)
        {
            return;
        }

        room.LeaveRoom(sessionID);
    }

    public void Handle_C_RoomChat(string sessionID, IMessage message)
    {
        RoomChatReq? packet = message as RoomChatReq;
        if (packet == null)
        {
            return;
        }

        // 방 채팅 처리
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

        // 채팅 메시지 전파
        room.SendChat(sessionID, packet.Message);
    }
}