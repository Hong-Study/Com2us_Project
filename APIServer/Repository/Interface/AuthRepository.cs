
using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class AuthRepository : IAuthRepository
{
    private IDbConnection _dbConn = null!;
    private readonly IConfiguration _config;
    private readonly SqlKata.Compilers.MySqlCompiler _compiler;
    private readonly QueryFactory _queryFactory;
    private readonly string _connectionString;

    public AuthRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("MySQL") == null ? 
                                throw new Exception("MySQL ConnectionString is null") 
                                : _config.GetConnectionString("MySQL")!;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
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
            System.Console.WriteLine("AccountRepository " + e.Message);
            return false;
        }
    }

    public async Task<bool> CheckUserAsync(int id)
    {
        try
        {
            UserGameData? result = await _queryFactory.Query("user_game_data").Where("id", id).FirstOrDefaultAsync<UserGameData>();
            if (result == null)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AccountRepository " + e.Message);
            return false;
        }
    }

    public async Task<UserGameData?> GetUserGameDataAsync(int id)
    {
        try
        {
            UserGameData? result = await _queryFactory.Query("user_game_data").Where("id", id).FirstOrDefaultAsync<UserGameData>();
            return result;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AccountRepository " + e.Message);
            return null;
        }
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_connectionString);
        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn.Close();
    }
}