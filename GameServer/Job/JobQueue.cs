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
        long tick = DateTime.Now.Ticks;

        while (_jobQueue.TryDequeue(out IJob? job))
        {
            if (job == null)
                break;

            job.Execute();
        }

        _lock.Unlock();
    }
}