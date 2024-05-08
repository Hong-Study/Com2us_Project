public interface IAttendanceCheckRepository
{
    public Task<bool> IsAttendanceCheck(string userId, DateTime date);
    public Task<bool> SetAttendanceCheck(UserAttendanceData data);
    public Task<bool> DeleteAttendanceChcek(string userId, DateTime data);
}