public interface IAuthRepository
{
    public Task<bool> CreateAccountAsync(UserDB accountDB);
    public Task<UserDB?> GetAccountAsync(string email);
    public Task<bool> UpdateAccountAsync(UserDB accountDB);
    public Task<bool> DeleteAccountAsync(string email);
    public Task<bool> CheckAccountAsync(string email);
}