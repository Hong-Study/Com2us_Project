using System.Data;
using MySqlConnector;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseManager : DataManager
{
    IDbConnection _dbConn = null!;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    readonly string _connectionString;

    DatabaseHandler _databaseHandler = null!;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;

        Initialize();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);

        InitHandler();
    }

    public void Initialize()
    {
        // DB 초기화
        _dbConn = new MySqlConnection(_connectionString);
        _dbConn.Open();
    }

    public void Release()
    {
        // DB 해제
        _dbConn.Close();
    }

    public override void InitHandler()
    {
        
    }
}