namespace GameServer;

public class AtomicLock
{
    int _locked = 0;
    public bool TryLock(int expected = 0, int newValue = 1) 
    { 
        if(Interlocked.CompareExchange(ref _locked, newValue, expected) == expected)
            return true;
            
        return false;
    }

    public void Unlock()
    {
        Interlocked.Exchange(ref _locked, 0);
    }
}