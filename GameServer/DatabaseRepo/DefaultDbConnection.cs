using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public abstract class DefaultDbConnection
{
    IDbConnection _dbConn = null!;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly string _connectionString;
    protected readonly QueryFactory _queryFactory;

    public DefaultDbConnection(string connectionString)
    {
        _connectionString = connectionString;
        
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