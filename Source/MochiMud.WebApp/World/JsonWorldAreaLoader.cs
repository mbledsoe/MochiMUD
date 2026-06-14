using System.Text.Json;
using System.Text.Json.Serialization;
using MochiMud.WebApp.Mobs;

namespace MochiMud.WebApp.World
{
    public class JsonWorldAreaLoader
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly IWorldFileStore worldFileStore;

        public JsonWorldAreaLoader(IWorldFileStore worldFileStore)
        {
            this.worldFileStore = worldFileStore;
        }

        public async Task<IReadOnlyDictionary<Guid, Room>> LoadRoomsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            return (await LoadAreaAsync(relativePath, cancellationToken)).Rooms.ToDictionary(room => room.Id);
        }

        public async Task<IReadOnlyCollection<Mob>> LoadMobsAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            var area = await LoadAreaAsync(relativePath, cancellationToken);
            var mobDefinitions = area.MobDefinitions.ToDictionary(
                mobDefinition => mobDefinition.Id,
                StringComparer.OrdinalIgnoreCase);

            return area.MobSpawns
                .Select(mobSpawn => CreateMob(mobSpawn, mobDefinitions))
                .ToArray();
        }

        private async Task<WorldAreaData> LoadAreaAsync(
            string relativePath,
            CancellationToken cancellationToken)
        {
            using var stream = await worldFileStore.OpenReadAsync(relativePath, cancellationToken);
            var area = await JsonSerializer.DeserializeAsync<WorldAreaData>(
                stream,
                SerializerOptions,
                cancellationToken);

            if (area is null)
            {
                throw new InvalidOperationException($"World area data file did not contain area data: {relativePath}");
            }

            return area;
        }

        private static Mob CreateMob(
            MobSpawnData mobSpawn,
            IReadOnlyDictionary<string, MobDefinitionData> mobDefinitions)
        {
            if (!mobDefinitions.TryGetValue(mobSpawn.MobDefinitionId, out var mobDefinition))
            {
                throw new InvalidOperationException($"Unknown mob definition referenced by spawn: {mobSpawn.MobDefinitionId}");
            }

            var mob = new Mob(mobDefinition.Name, mobSpawn.RoomId)
            {
                BareHandDamage = mobDefinition.BareHandDamage,
                IsAggressive = mobDefinition.IsAggressive,
            };

            return mob;
        }
    }
}
