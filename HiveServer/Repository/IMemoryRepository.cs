public interface IMemoryRepository
{
    public Task<string?> GetAccessToken(long user_id);
    public Task<bool> SetAccessToken(long user_id, string token);
    public Task<bool> DeleteAccessToken(long user_id);
}