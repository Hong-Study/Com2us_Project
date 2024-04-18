public class AttendanceCheckService : IAttendanceCheckService
{
    public IAttendanceCheckRepository _repo;
    public AttendanceCheckService(IAttendanceCheckRepository repo)
    {
        _repo = repo;
    }

    public AttendanceCheckRes AttendanceCheck(AttendanceCheckReq req)
    {
        // 유저가 테이블에 존재한다고 생각하고
        // 유저의 출석 여부를 체크하는 로직을 작성한다.
        // 이후에는 해당 유저의 출석 여부를 반환한다.
        return new AttendanceCheckRes
        {
            IsSuccess = true
        };
    }
}