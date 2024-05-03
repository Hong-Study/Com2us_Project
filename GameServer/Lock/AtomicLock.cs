namespace GameServer;

public class AtomicLock
{
    Int32 _locked = 0;
    public bool TryLock(Int32 expected = 0, Int32 newValue = 1) 
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