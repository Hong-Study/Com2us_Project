using Common;

namespace GameServer;

public partial class PacketHandler
{
    public void Handle_C_GameReady(string sessionID, IMessage message)
    {
        CGameReadyReq? packet = message as CGameReadyReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoom(sessionID);
        if (room == null)
        {
            return;
        }

        room.Push(() => room.GameReady(sessionID, packet.IsReady));
    }

    public void Handle_C_GameStart(string sessionID, IMessage message)
    {
        CGameStartRes? packet = message as CGameStartRes;
        if (packet == null)
        {
            return;
        }

    }

    public void Handle_C_GamePut(string sessionID, IMessage message)
    {
        CGamePutReq? packet = message as CGamePutReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoom(sessionID);
        if (room == null)
        {
            return;
        }

        room.Push(() => room.GamePut(sessionID, packet.X, packet.Y));
    }

    public void Handle_C_TurnChange(string sessionID, IMessage message)
    {
        CTurnChangeRes? packet = message as CTurnChangeRes;
        if (packet == null)
        {
            return;
        }
    }

    public void Handle_C_GameEnd(string sessionID, IMessage message)
    {
        CGameEndRes? packet = message as CGameEndRes;
        if (packet == null)
        {
            return;
        }
    }

    public void Handle_C_GameCancle(string sessionID, IMessage message)
    {
        CGameCancleRes? packet = message as CGameCancleRes;
        if (packet == null)
        {
            return;
        }

    }
}