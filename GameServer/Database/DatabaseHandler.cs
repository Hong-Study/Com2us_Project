using SqlKata.Execution;

namespace GameServer;

public class DatabaseHandler
{
    readonly QueryFactory _queryFactory;

    public DatabaseHandler(QueryFactory queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public void Init()
    {
        // DB 초기화
    }
}