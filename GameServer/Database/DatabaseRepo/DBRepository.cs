using Common;
using SqlKata.Execution;

namespace GameServer;

public class DBRepository
{
    SuperSocket.SocketBase.Logging.ILog Logger = null!;

    public DBRepository() { }

    public void InitLogger(SuperSocket.SocketBase.Logging.ILog logger)
    {
        Logger = logger;
    }

    public record GetUserGameDataResult(ErrorCode errorCode, UserData? userData);
    public GetUserGameDataResult GetUserGameDataAsync(DBConnector connector, string userId)
    {
        try
        {
            UserGameData? result = connector.QueryFactory.Query("user_game_data")
                                            .Where("user_id", userId)
                                            .FirstOrDefault<UserGameData>();


            if (result == null)
            {
                return MakeUserGameDataResult(ErrorCode.NOT_FOUND_USER_INFO, null);
            }
            else
            {
                return MakeUserGameDataResult(ErrorCode.NONE, new UserData
                {
                    UserID = result.user_id,
                    NickName = result.user_name,
                    Win = result.win,
                    Lose = result.lose,
                    Level = result.level
                });
            }
        }
        catch (Exception e)
        {
            Logger.Error("UserRepository " + e.Message);
            return MakeUserGameDataResult(ErrorCode.EXCEPTION_USER_DATABASE, null);
        }
    }

    public ErrorCode UpdateUserWinLoseAsync(DBConnector connector, string userId, Int32 win, Int32 lose)
    {
        try
        {
            var result = connector.QueryFactory.Query("user_game_data")
                            .Where("user_id", userId)
                            .Update(new
                            {
                                win = win,
                                lose = lose
                            });

            if (result == 0)
            {
                return ErrorCode.FAILED_DATA_UPDATE;
            }
            else
            {
                return ErrorCode.NONE;
            }
        }
        catch (Exception e)
        {
            Logger.Error("UserRepository " + e.Message);
            return ErrorCode.EXCEPTION_USER_DATABASE;
        }
    }

    public GetUserGameDataResult MakeUserGameDataResult(ErrorCode errorCode, UserData? userData)
    {
        return new GetUserGameDataResult(errorCode, userData);
    }
}