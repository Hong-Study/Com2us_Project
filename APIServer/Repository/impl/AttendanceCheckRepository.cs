using SqlKata.Execution;

public class AttendanceCheckRepository : DefaultDbConnection, IAttendanceCheckRepository
{
    public AttendanceCheckRepository(IConfiguration config) : base(config)
    {
        
    }

    public async Task<bool> IsAttendanceCheck(int id, DateTime date)
    {
        var data = await _queryFactory.Query("attendance_check_data").Where("user_id", id).Where("date", date).FirstOrDefaultAsync<UserAttendanceData>();

        return true;
    }

    public async Task<bool> SetAttendanceCheck(int id, DateTime date)
    {
        var data = await _queryFactory.Query("attendance_check_data").Where("user_id", id).Where("date", date).FirstOrDefaultAsync<UserAttendanceData>();

        return true;
    }
}