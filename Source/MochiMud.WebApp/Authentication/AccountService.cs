using System.Text.RegularExpressions;

namespace MochiMud.WebApp.Authentication
{
    public partial class AccountService
    {
        private const int MinPasswordLength = 8;

        private readonly IAccountStore accountStore;
        private readonly IPasswordHasher passwordHasher;

        public AccountService(IAccountStore accountStore, IPasswordHasher passwordHasher)
        {
            this.accountStore = accountStore;
            this.passwordHasher = passwordHasher;
        }

        public async Task<RegisterResult> RegisterAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            username = (username ?? string.Empty).Trim();
            password ??= string.Empty;

            if (!UsernameRegex().IsMatch(username) || password.Length < MinPasswordLength)
            {
                return new RegisterResult(RegisterOutcome.Invalid, null);
            }

            var passwordHash = passwordHasher.Hash(password);

            var account = new Account(
                Guid.NewGuid(),
                username,
                Account.Normalize(username),
                passwordHash.Hash,
                passwordHash.Salt,
                passwordHash.Iterations,
                DateTimeOffset.UtcNow);

            var created = await accountStore.TryCreateAsync(account, cancellationToken);

            return created
                ? new RegisterResult(RegisterOutcome.Success, account)
                : new RegisterResult(RegisterOutcome.UsernameTaken, null);
        }

        public async Task<Account?> ValidateCredentialsAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            username = (username ?? string.Empty).Trim();
            password ??= string.Empty;

            var account = await accountStore.FindByUsernameAsync(username, cancellationToken);

            if (account is null)
            {
                return null;
            }

            var verified = passwordHasher.Verify(
                password,
                account.PasswordHash,
                account.PasswordSalt,
                account.Iterations);

            return verified ? account : null;
        }

        [GeneratedRegex("^[A-Za-z0-9_]{3,20}$")]
        private static partial Regex UsernameRegex();
    }

    public enum RegisterOutcome
    {
        Success,
        UsernameTaken,
        Invalid,
    }

    public sealed record RegisterResult(RegisterOutcome Outcome, Account? Account);
}
