public class MatchingData
{
    public PublishType Type { get; set; }
    public Int64 MatchID { get; set; }
    public MatchingUserData? MatchingUserData { get; set; }
    public MatchingServerInfo? MatchingServerInfo { get; set; }
}

public class MatchingUserData
{
    public string FirstUserID { get; set; } = null!;
    public string SecondUserID { get; set; } = null!;
}

public enum PublishType
{
    Matching,
    Complete
}