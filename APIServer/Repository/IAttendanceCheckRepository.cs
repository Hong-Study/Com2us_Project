public interface IAttendanceCheckRepository
{
    public Task<bool> IsAttendanceCheck(Int64 userId, DateTime date);
    public Task<bool> SetAttendanceCheck(UserAttendanceData data);
    public Task<bool> DeleteAttendanceChcek(Int64 userId, DateTime data);
}