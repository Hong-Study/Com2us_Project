public interface IAttendanceCheckRepository
{
    public Task<bool> IsAttendanceCheck(int id, DateTime date);   
    public Task<bool> SetAttendanceCheck(int id, DateTime date);
}