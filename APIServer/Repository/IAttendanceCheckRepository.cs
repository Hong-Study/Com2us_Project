public interface IAttendanceCheckRepository
{
    public Task<bool> IsAttendanceCheck(long userId, DateTime date);
    public Task<bool> SetAttendanceCheck(UserAttendanceData data);
    public Task<bool> DeleteAttendanceChcek(long userId, DateTime data);
}