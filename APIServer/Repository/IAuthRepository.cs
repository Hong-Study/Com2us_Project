public interface IAuthRepository
{
    // 취소 잘 처리하기
    Task<bool> CreateUserAsync(UserGameData data);
    Task<bool> CheckUserAsync(Int64 userId);
    Task<UserGameData?> GetUserGameDataAsync(Int64 userId);
}