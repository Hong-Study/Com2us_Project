
using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class AuthRepository : DefaultDbConnection, IAuthRepository
{
    ILogger<AuthRepository> _logger;
    public AuthRepository(IConfiguration config, ILogger<AuthRepository> logger) : base(config)
    {
        _logger = logger;
    }

    public async Task<bool> CreateUserAsync(UserGameData data)
    {
        try
        {
            int count = await _queryFactory.Query("user_game_data").InsertAsync(data);
            return count == 0 ? false : true;
        }
        catch (Exception e)
        {
            _logger.LogInformation("AuthRepository[CreateUseAsync] " + e.Message);
            return false;
        }
    }

    public async Task<bool> CheckUserAsync(int userId)
    {
        try
        {
            // 값을 다 가져오지 않고 하나만 가져와도 된다.
            UserIdData? result = await _queryFactory.Query("user_game_data")
                                        .Select("user_id")
                                        .Where("user_id", userId)
                                        .FirstOrDefaultAsync<UserIdData>();
            if (result == null)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return false;
        }
    }

    public async Task<UserGameData?> GetUserGameDataAsync(int userId)
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
}