public interface IAttendanceCheckRepository
{
    public Task<bool> IsAttendanceCheck(int userId, DateTime date);   
    public Task<bool> SetAttendanceCheck(UserAttendanceData data);
}