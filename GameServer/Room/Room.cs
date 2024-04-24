using MemoryPack;

namespace GameServer;

public class Room
{
    List<ClientSession> clients = new List<ClientSession>();
    public Room() { }

    public void EnterRoom(ClientSession session)
    {
        clients.Add(session);
    }
}