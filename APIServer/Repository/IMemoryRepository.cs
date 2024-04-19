public interface IMemoryRepository
{
    public Task<string?> GetAccessToken(string id);
    public Task<bool> SetAccessToken(string userId, string token);
    public Task<bool> DeleteAccessToken(string id);
}