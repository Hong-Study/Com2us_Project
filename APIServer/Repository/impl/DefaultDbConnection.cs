using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class DefaultDbConnection
{
    IDbConnection _dbConn = null!;
    readonly IConfiguration _config;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly string _connectionString;
    protected readonly QueryFactory _queryFactory;

    public DefaultDbConnection(IConfiguration config)
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