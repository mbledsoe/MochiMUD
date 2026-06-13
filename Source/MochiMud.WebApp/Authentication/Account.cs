namespace MochiMud.WebApp.Authentication
{
    public sealed record Account(
        Guid Id,
        string Username,
        string NormalizedUsername,
        string PasswordHash,
        string PasswordSalt,
        int Iterations,
        DateTimeOffset CreatedAt)
    {
        public static string Normalize(string username)
        {
            return username.Trim().ToLowerInvariant();
        }
    }
}
