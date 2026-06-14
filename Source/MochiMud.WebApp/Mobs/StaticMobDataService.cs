using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Mobs
{
    public class StaticMobDataService : IMobDataService
    {
        private const string AreasManifestPath = "World/Data/areas.json";

        private readonly IReadOnlyCollection<Mob> mobs;

        public StaticMobDataService(
            JsonWorldAreaManifestLoader jsonWorldAreaManifestLoader,
            JsonWorldAreaLoader jsonWorldAreaLoader)
        {
            mobs = CreateMobsAsync(jsonWorldAreaManifestLoader, jsonWorldAreaLoader)
                .GetAwaiter()
                .GetResult();
        }

        public Task<IReadOnlyCollection<Mob>> GetMobsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(mobs);
        }

        public Task<IReadOnlyCollection<Mob>> GetMobsInRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var mobsInRoom = mobs
                .Where(mob => mob.CurrentRoomId == roomId)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Mob>>(mobsInRoom);
        }

        private static async Task<IReadOnlyCollection<Mob>> CreateMobsAsync(
            JsonWorldAreaManifestLoader jsonWorldAreaManifestLoader,
            JsonWorldAreaLoader jsonWorldAreaLoader)
        {
            var mobs = new List<Mob>();

            foreach (var areaPath in await jsonWorldAreaManifestLoader.LoadAreasAsync(AreasManifestPath))
            {
                mobs.AddRange(await jsonWorldAreaLoader.LoadMobsAsync(areaPath));
            }

            return mobs;
        }
    }
}
