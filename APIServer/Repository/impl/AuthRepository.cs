
using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class AuthRepository : DefaultDbConnection, IAuthRepository
{
    // IDbConnection _dbConn = null!;
    // readonly IConfiguration _config;
    // readonly SqlKata.Compilers.MySqlCompiler _compiler;
    // readonly QueryFactory _queryFactory;
    // readonly string _connectionString;

    public AuthRepository(IConfiguration config) : base(config)
    {
        
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
            System.Console.WriteLine("AuthRepository " + e.Message);
            return false;
        }
    }

    public async Task<bool> CheckUserAsync(int userId)
    {
        try
        {
            UserGameData? result = await _queryFactory.Query("user_game_data").Where("user_id", userId).FirstOrDefaultAsync<UserGameData>();
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
            UserGameData? result = await _queryFactory.Query("user_game_data").Where("user_id", userId).FirstOrDefaultAsync<UserGameData>();
            return result;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return null;
        }
    }
}