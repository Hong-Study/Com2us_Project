using System;
using Common;

namespace CSCommon;

public abstract class DefaultRes
{
    public ErrorCode ErrorCode { get; set; } = ErrorCode.NONE;
}

public class HiveLoginReq
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class HiveLoginRes : DefaultRes
{
    public long Id { get; set; }
    public string Token { get; set; }
}

public class ApiLoginReq
{
    public Int64 UserId { get; set; }

    public string Token { get; set; } = null!;
}

public class ApiLoginRes : DefaultRes
{
    public UserGameData GameData { get; set; }
    public string GameServerAddress { get; set; }
    public Int32 GameServerPort { get; set; }
}

public class UserGameData
{
    public Int64 user_id { get; set; }
    public string user_name { get; set; } = null!;
    public int level { get; set; }
    public int exp { get; set; }
    public int gold { get; set; }
    public int win { get; set; }
    public int lose { get; set; }
    public DateTime? created_at { get; set; }
}

public class HiveRegisterReq
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class HiveRegisterRes : DefaultRes
{
    public bool IsSuccess { get; set; }
}