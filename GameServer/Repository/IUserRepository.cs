public interface IUserRepository
{
    Task<UserGameData?> GetUserGameDataAsync(Int64 userId);
    Task<bool> UpdateUserWinLoseAsync(Int64 userId, Int32 win, Int32 lose);
}