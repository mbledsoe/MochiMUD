namespace MochiMud.WebApp.Authentication
{
    public interface IAccountStore
    {
        Task<Account?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

        Task<bool> TryCreateAsync(Account account, CancellationToken cancellationToken = default);
    }
}
