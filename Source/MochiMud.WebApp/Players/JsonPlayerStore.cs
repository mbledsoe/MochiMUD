using System.Text.Json;
using System.Text.Json.Serialization;
using MochiMud.WebApp.Storage;

namespace MochiMud.WebApp.Players
{
    public class JsonPlayerStore : IPlayerStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly string storageDirectory;
        private readonly string nameDirectory;

        public JsonPlayerStore(DataDirectoryProvider dataDirectoryProvider)
        {
            storageDirectory = dataDirectoryProvider.GetDirectory("players");
            nameDirectory = dataDirectoryProvider.GetDirectory("players", "names");
        }

        public async Task SaveAsync(Player player, CancellationToken cancellationToken = default)
        {
            var playerData = new PlayerData(
                player.Id,
                player.Name,
                player.HitPoints,
                player.Class,
                player.Level,
                player.ExperiencePoints,
                player.Mana,
                player.AutoExits);

            var filePath = GetFilePath(player.Id);
            var tempFilePath = filePath + ".tmp";

            await using (var stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(stream, playerData, SerializerOptions, cancellationToken);
            }

            File.Move(tempFilePath, filePath, overwrite: true);
        }

        public async Task<PlayerData?> LoadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filePath = GetFilePath(id);

            if (!File.Exists(filePath))
            {
                return null;
            }

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return await JsonSerializer.DeserializeAsync<PlayerData>(stream, SerializerOptions, cancellationToken);
        }

        public async Task<bool> TryReserveNameAsync(string name, Guid accountId, CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLowerInvariant();
            var filePath = Path.Combine(nameDirectory, $"{normalizedName}.json");

            try
            {
                await using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

                await JsonSerializer.SerializeAsync(stream, new ReservedName(name, accountId), SerializerOptions, cancellationToken);

                return true;
            }
            catch (IOException) when (File.Exists(filePath))
            {
                return false;
            }
        }

        private string GetFilePath(Guid id)
        {
            return Path.Combine(storageDirectory, $"{id}.json");
        }

        private sealed record ReservedName(string Name, Guid AccountId);
    }
}
