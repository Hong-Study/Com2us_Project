public interface IAuthRepository
{
    // 취소 잘 처리하기
    Task<bool> CreateUserAsync(UserGameData data);
    Task<bool> CheckUserAsync(string userId);
    Task<UserGameData?> GetUserGameDataAsync(string userId);
    Task<UserNameData?> GetUserNameDataAsync(string userId);
}