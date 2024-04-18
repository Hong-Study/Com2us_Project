public interface IAuthRepository
{
    // 취소 잘 처리하기
    Task<bool> CreateUserAsync(UserGameData data);
    Task<bool> CheckUserAsync(int userId);
    Task<UserGameData?> GetUserGameDataAsync(int userId);
}