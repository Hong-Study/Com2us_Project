using SqlKata.Execution;

public class AttendanceCheckRepository : DefaultDbConnection, IAttendanceCheckRepository
{
    ILogger<AttendanceCheckRepository> _logger;
    public AttendanceCheckRepository(IConfiguration config, ILogger<AttendanceCheckRepository> logger) : base(config)
    {
        _logger = logger;
    }

    public async Task<bool> IsAttendanceCheck(int userId, DateTime date)
    {
        try
        {
            UserIdData? data = await _queryFactory.Query("user_attendance_data")
                                        .Select("user_id")
                                        .Where("user_id", userId)
                                        .Where("attendance_date", date)
                                        .FirstOrDefaultAsync<UserIdData>();
            if (data == null)
                return false;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("AttendanceCheckRepository IsChcek " + e.Message);
            return false;
        }
    }

    public async Task<bool> SetAttendanceCheck(UserAttendanceData data)
    {
        try
        {
            int result = await _queryFactory.Query("user_attendance_data").InsertAsync(data);
            if (result == 0)
                return false;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("AttendanceCheckRepository " + e.Message);
            return false;
        }
    }
}