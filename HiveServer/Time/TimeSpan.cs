public class TimeSpanUtils
{
    public static TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
    }

    public static TimeSpan TicketKeyTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
        // return TimeSpan.FromSeconds(RediskeyExpireTime.TicketKeyExpireSecond);
    }

    public static TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromMinutes(100);
        // return TimeSpan.FromSeconds(RediskeyExpireTime.NxKeyExpireSecond);
    }
}