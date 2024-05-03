using System.Data;
using MySqlConnector;
using SqlKata.Execution;

namespace GameServer;

public class DatabaseManager : DataManager
{
    IUserRepository userRepository;

    public DatabaseManager(string connectionString)
    {
        userRepository = new UserRepository(connectionString);

        InitHandler();
    }

    public override void InitHandler()
    {
        throw new NotImplementedException();
    }
}