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
        _connectionString = _config.GetConnectionString("MySQL") == null ? throw new Exception("MySQL ConnectionString is null") : _config.GetConnectionString("MySQL")!;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<bool> CheckAccountAsync(string email)
    {
        try
        {
            var userInfo = await _queryFactory.Query("users").Where("email", email).FirstOrDefaultAsync<UserDB>();
            if (userInfo == null)
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

    public async Task<bool> CreateAccountAsync(UserDB accountDB)
    {
        try
        {
            int count = await _queryFactory.Query("users").InsertAsync(accountDB);

            return count == 0 ? false : true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return false;
        }
    }

    public async Task<UserDB?> GetAccountAsync(string email)
    {
        try
        {
            UserDB? userDB = await _queryFactory.Query("users").Where("email", email)
                                        .FirstOrDefaultAsync<UserDB>();

            if (userDB == null)
            {
                return null;
            }

            return userDB;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("AuthRepository " + e.Message);
            return null;
        }
    }

    public Task<bool> UpdateAccountAsync(UserDB accountDB)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAccountAsync(string email)
    {
        throw new NotImplementedException();
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