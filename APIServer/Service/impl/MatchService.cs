using System.Collections.Concurrent;

public class MatchService : IMatchService
{
    ConcurrentDictionary<string, GameServerInfo> gameServers = new ConcurrentDictionary<string, GameServerInfo>();

    public void AddGameServer(string address, Int32 port)
    {
        GameServerInfo gameServerInfo = new GameServerInfo();
        gameServerInfo.GameServerAddress = address;
        gameServerInfo.GameServerPort = port;

        string key = gameServerInfo.GameServerAddress + ":" + gameServerInfo.GameServerPort;
        gameServers.TryAdd(key, gameServerInfo);
    }

    public void RemoveGameServer(string serverKey)
    {
        gameServers.TryRemove(serverKey, out _);
    }

    public record MatchMachkingResult(ErrorCode errorCode, string? gameServerAddress = null, Int32 gameServerPort = 0);
    public MatchMachkingResult GetGameServer(string serverKey)
    {
        gameServers.TryGetValue(serverKey, out GameServerInfo? gameServerInfo);
        return new MatchMachkingResult(ErrorCode.NONE, gameServerInfo?.GameServerAddress, gameServerInfo?.GameServerPort ?? 0);
    }
}