using System.Runtime.CompilerServices;

public class MatchingData
{
    public PublishType Type { get; set; }
    public Int64 MatchID { get; set; }
    public string FirstUserID { get; set; } = null!;
    public string SecondUserID { get; set; } = null!;
}
public enum PublishType
{
    Matching,
    Complete
}

public class CompleteMatchingData
{
    public Int64 MatchID { get; set; }
    public string FirstUserID { get; set; } = null!;
    public string SecondUserID { get; set; } = null!;
    public MatchingServerInfo ServerInfo { get; set; } = null!;
}

public class MatchingServerInfo
{
    public string ServerAddress { get; set; } = null!;
    public Int32 Port { get; set; }
    public Int32 RoomNumber { get; set; }
}