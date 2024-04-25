using Microsoft.VisualBasic;

namespace GameServer;

public interface IJob
{
    void Execute();
}

public class Job : IJob
{
    Action _action;

    public Job(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action.Invoke();
    }
}