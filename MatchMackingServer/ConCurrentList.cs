using System.Diagnostics.CodeAnalysis;

public class ConcurrentList<T>
{
    object _lock = new object();

    List<T> _list = new List<T>();

    public void Add(T item)
    {
        lock (_lock)
        {
            _list.Add(item);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _list.Remove(item);
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _list.Contains(item);
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _list.Count;
            }
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _list.Clear();
        }
    }

    public bool TryGetFirstAndDelete([NotNullWhen(true)] out T? item)
    {
        lock (_lock)
        {
            item = _list.FirstOrDefault();
            if (item != null)
            {
                _list.RemoveAt(0);
                return true;
            }

            return false;
        }
    }
}