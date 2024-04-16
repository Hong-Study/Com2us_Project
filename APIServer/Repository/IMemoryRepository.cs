public interface IMemoryRepository
{
    public Task<string?> GetAccessToken(string token);
    public Task<bool> SetAccessToken(int userId, string token);
    public Task<bool> DeleteAccessToken(string token);
}