using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class AuthRepository : IAuthRepository
{
    ILogger<AuthRepository> _logger;
    IDbConnection _dbConn = null!;
    readonly IConfiguration _config;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    readonly string _connectionString;

    public AuthRepository(IConfiguration config, ILogger<AuthRepository> logger)
    {
        _logger = logger;
        _config = config;

        // 코드 심플?
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

    public async Task<bool> CheckAccountAsync(string email)
    {
        try
        {
            UserData? userData = await _queryFactory.Query("user_data")
                                        .Where("user_id", email)
                                        .FirstOrDefaultAsync<UserData>();
            if (userData == null)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return false;
        }
    }

    public async Task<bool> CreateAccountAsync(UserData accountDB)
    {
        // 에러 코드로 던지는게 좋을 수도 있다.
        // 3가지의 경우이기 때문에
        try
        {
            int count = await _queryFactory.Query("user_data").InsertAsync(accountDB);
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return false;
        }
    }

    public async Task<UserData?> GetAccountAsync(string email)
    {
        try
        {
            UserData? userData = await _queryFactory.Query("user_data")
                                        .Where("user_id", email)
                                        .FirstOrDefaultAsync<UserData>();

            return userData;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return null;
        }
    }

    public Task<bool> UpdateAccountAsync(UserData accountDB)
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