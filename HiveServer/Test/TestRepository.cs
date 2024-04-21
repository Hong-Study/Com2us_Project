using System.Data;
using MySqlConnector;
using SqlKata.Execution;

public class TestRepository : ITestRepository
{
    private IDbConnection _dbConn = null!;
    private readonly IConfiguration _config;
    private readonly SqlKata.Compilers.MySqlCompiler _compiler;
    private readonly QueryFactory _queryFactory;
    private readonly string _connectionString;

    public TestRepository(IConfiguration config)
    {
        _config = config;

        // 코드 심플?
        _connectionString = _config.GetConnectionString("MySQL") == null ?
                    throw new Exception("MySQL ConnectionString is null")
                    : _config.GetConnectionString("MySQL")!;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public async Task<TestData> GetTestData()
    {
        TestData testData = await _queryFactory.Query("test_table")
            .Join("user_data", "test_table.first_id", "user_data.id")
            .FirstOrDefaultAsync<TestData>();

        return testData;
    }

    public async Task<bool> InsertTestData(TestData testData)
    {
        try
        {
            int count = await _queryFactory.Query("test_table").InsertAsync(testData);
            return count == 1;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("TestRepository " + e.Message);
            return false;
        }
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

public class TestData
{
    public long first_id { get; set; }
    public long second_id { get; set; }
}