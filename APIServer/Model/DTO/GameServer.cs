public class StartGameServerReq
{
    public string GameServerAddress { get; set; } = null!;
    public Int32 GameServerPort { get; set; }
}

public class StartGameServerRes : DefaultRes
{

}

public class EndGameServerReq
{
    public string GameServerAddress { get; set; } = null!;
    public Int32 GameServerPort { get; set; }
}

public class EndGameServerRes : DefaultRes
{

}