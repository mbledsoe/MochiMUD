namespace MochiMud.WebApp.Authentication
{
    public interface IPasswordHasher
    {
        PasswordHash Hash(string password);

        bool Verify(string password, string hash, string salt, int iterations);
    }

    public sealed record PasswordHash(string Hash, string Salt, int Iterations);
}
