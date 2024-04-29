using System.Collections.Concurrent;

namespace GameServer;

public interface IJobQueue
{
    public void Push(Action job);
}

public class JobQueue
{
    ConcurrentQueue<IJob> _jobQueue = new ConcurrentQueue<IJob>();
    AtomicLock _lock = new AtomicLock();

    public void Push(Action job)
    {
        _jobQueue.Enqueue(new Job(job));

        if (_lock.TryLock())
        {
            Run();
        }
    }

    // 이 함수는 하나의 스레드에서만 실행되어야 함.
    // 고로 Lock을 사용
    void Run()
    {
        // job == null인 코드를 처리하는 과정에서 누군가가 Push를 한다면? 처리가 안되고 좀비 패킷으로 남아있을 수도 있다.
        // 따라서 해당 처리를 제대로 해 줘야함.
        while (_jobQueue.TryDequeue(out IJob? job))
        {
            if (job == null)
            {
                _lock.Unlock();
                break;
            }

            job.Execute();
        }
    }
}