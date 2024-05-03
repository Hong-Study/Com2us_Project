namespace GameServer;

public class ServerOption
{
    public Int32 ServerUniqueID { get; set; }
    public string Name { get; set; } = null!;
    public Int32 MaxConnectionNumber { get; set; }
    public Int32 Port { get; set; }
    public Int32 MaxRequestLength { get; set; }
    public Int32 ReceiveBufferSize { get; set; }
    public Int32 SendBufferSize { get; set; }

    public Int32 MaxUserCount { get; set; } = 0;
    
    public Int32 MaxRoomCount { get; set; } = 0;
    public Int32 MaxRoomUserCount { get; set; } = 0;
    public Int32 RoomStartNumber { get; set; } = 0;     

    public Int32 MaxRoomCheckCount { get; set; }
    public Int32 MaxSessionCheckCount { get; set; }
    public Int32 MaxHeartBeatCheckCount { get; set; }

    public string DatabaseConnectionString { get; set; } = null!;
    public string MemoryConnectionString { get; set; } = null!; 

    public Int32 SessionTimeoutMilliSeconds { get; set; }
    public Int32 HeartBeatMilliSeconds { get; set; }
    public Int32 RoomCheckMilliSeconds { get; set; }
    
    public Int32 SessionTimeoutTimerMilliSeconds { get; set; }
    public Int32 RoomCheckTimerMilliSeconds { get; set; }
    public Int32 HeartBeatTimerMilliSeconds { get; set; }

    public Int32 OmokGameTurnTimeoutSeconds { get; set; }
    public Int32 OmokGameTurnTimeoutCount { get; set; }
    public Int32 OmokGameMaxGameTimeMinute { get; set; }
}    
