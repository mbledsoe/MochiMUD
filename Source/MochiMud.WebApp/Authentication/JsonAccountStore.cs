using System.Text.Json;
using MochiMud.WebApp.Storage;

namespace MochiMud.WebApp.Authentication
{
    public class JsonAccountStore : IAccountStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
        };

        private readonly string storageDirectory;

        public JsonAccountStore(DataDirectoryProvider dataDirectoryProvider)
        {
            storageDirectory = dataDirectoryProvider.GetDirectory("accounts");
        }

        public async Task<Account?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(username);

            if (!File.Exists(filePath))
            {
                return null;
            }

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return await JsonSerializer.DeserializeAsync<Account>(stream, SerializerOptions, cancellationToken);
        }

        public async Task<bool> TryCreateAsync(Account account, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(account.NormalizedUsername);

            try
            {
                await using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

                await JsonSerializer.SerializeAsync(stream, account, SerializerOptions, cancellationToken);

                return true;
            }
            catch (IOException) when (File.Exists(filePath))
            {
                return false;
            }
        }

        private string GetFilePath(string username)
        {
            var normalized = Account.Normalize(username);

            return Path.Combine(storageDirectory, $"{normalized}.json");
        }
    }
}
