public interface IMatchService
{
    void AddGameServer(string address, Int32 port);
    void RemoveGameServer(string serverKey);
}