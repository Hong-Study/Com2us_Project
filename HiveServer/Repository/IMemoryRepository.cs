public interface IMemoryRepository
{
    public Task<string?> GetAccessToken(int user_id);
    public Task<bool> SetAccessToken(int user_id, string token);
    public Task<bool> DeleteAccessToken(int user_id);
}