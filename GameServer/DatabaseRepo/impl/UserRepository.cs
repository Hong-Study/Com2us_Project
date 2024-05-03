
using SqlKata.Execution;

public class UserRepository : DefaultDbConnection, IUserRepository
{
    public UserRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<UserGameData?> GetUserGameDataAsync(Int64 userId)
    {
        try
        {
            UserGameData? result = await _queryFactory.Query("user_game_data")
                                            .Where("user_id", userId)
                                            .FirstOrDefaultAsync<UserGameData>();
            return result;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return null;
        }
    }

    public async Task<bool> UpdateUserWinLoseAsync(Int64 userId, Int32 win, Int32 lose)
    {
        try
        {
            await _queryFactory.Query("user_game_data")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    win = win,
                    lose = lose
                });
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return false;
        }
    }
}