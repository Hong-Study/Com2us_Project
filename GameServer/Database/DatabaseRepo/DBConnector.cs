using System.Data;
using MySqlConnector;
using SqlKata.Execution;

namespace GameServer;

public class DBConnector
{
    IDbConnection _dbConn = null!;
    SqlKata.Compilers.MySqlCompiler _compiler = null!;

    public QueryFactory QueryFactory { get; private set; } = null!;

    public DBConnector(string connectionString)
    {
        _dbConn = new MySqlConnection(connectionString);

        Open();
    }

    public void ReConnect()
    {
        if (_dbConn.State == ConnectionState.Closed)
        {
            _dbConn.Open();
        }
    }

    public void Close()
    {
        _dbConn.Close();
    }

    void Open()
    {
        _dbConn.Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        QueryFactory = new QueryFactory(_dbConn, _compiler);
    }
}