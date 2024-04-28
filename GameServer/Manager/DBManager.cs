using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class DBManager
{
    IDbConnection _dbConn = null!;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    readonly string _connectionString;

    public DBManager(string connectionString)
    {
        _connectionString = connectionString;

        Initialize();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
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

    public void UpdateUser(string id, string pw)
    {
        // 유저 정보 DB에서 수정
    }

    public void SelectUser(string id)
    {
        // 유저 정보 DB에서 조회
    }
}