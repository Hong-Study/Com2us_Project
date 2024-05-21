using Common;

namespace GameServer;

public partial class PacketHandler
{
    public void HandleCGameReady(string sessionID, IMessage message)
    {
        CGameReadyReq? packet = message as CGameReadyReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoom<SGameReadyRes>(sessionID, PacketType.RES_S_GAME_READY);
        if (room == null)
        {
            return;
        }

        room.GameReady(sessionID, packet.IsReady);
    }

    public void HandleCGameStart(string sessionID, IMessage message)
    {
        CGameStartRes? packet = message as CGameStartRes;
        if (packet == null)
        {
            return;
        }
    }

    public void HandleCGamePut(string sessionID, IMessage message)
    {
        CGamePutReq? packet = message as CGamePutReq;
        if (packet == null)
        {
            return;
        }

        var room = GetRoom<SGamePutRes>(sessionID, PacketType.RES_S_GAME_PUT);
        if (room == null)
        {
            return;
        }

       room.GamePut(sessionID, packet.X, packet.Y);
    }

    public void HandleCTurnChange(string sessionID, IMessage message)
    {
        CTurnChangeRes? packet = message as CTurnChangeRes;
        if (packet == null)
        {
            return;
        }
    }

    public void HandleCGameEnd(string sessionID, IMessage message)
    {
        CGameEndRes? packet = message as CGameEndRes;
        if (packet == null)
        {
            return;
        }
    }

    public void HandleCGameCancle(string sessionID, IMessage message)
    {
        CGameCancleRes? packet = message as CGameCancleRes;
        if (packet == null)
        {
            return;
        }

    }
}