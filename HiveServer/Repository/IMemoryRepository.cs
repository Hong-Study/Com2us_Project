public interface IMemoryRepository
{
    public Task<string?> GetAccessToken(string userID);
    public Task<bool> SetAccessToken(string userID, string token);
    public Task<bool> DeleteAccessToken(string userID);
}