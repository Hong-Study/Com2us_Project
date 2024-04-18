public interface IAuthRepository
{
    public Task<bool> CreateAccountAsync(UserData accountDB);
    public Task<UserData?> GetAccountAsync(string email);
    public Task<bool> UpdateAccountAsync(UserData accountDB);
    public Task<bool> DeleteAccountAsync(string email);
    public Task<bool> CheckAccountAsync(string email);
}