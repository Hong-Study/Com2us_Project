using SqlKata.Execution;

namespace GameServer;

public class DatabaseManager : DataManager
{
    
    readonly string _connectionString;

    DatabaseHandler _databaseHandler;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;

        Initialize();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        var queryFactory = new QueryFactory(_dbConn, _compiler);
        _databaseHandler = new DatabaseHandler(queryFactory);
        InitHandler();
    }

    public void Initialize()
    {
        // DB 초기화
        
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