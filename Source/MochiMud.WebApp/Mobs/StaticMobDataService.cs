namespace MochiMud.WebApp.Mobs
{
    public class StaticMobDataService : IMobDataService
    {
        private static readonly IReadOnlyCollection<Mob> Mobs = ChessboardMobBuilder.CreateMobs();

        public Task<IReadOnlyCollection<Mob>> GetMobsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Mobs);
        }

        public Task<IReadOnlyCollection<Mob>> GetMobsInRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var mobs = Mobs
                .Where(mob => mob.CurrentRoomId == roomId)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Mob>>(mobs);
        }
    }
}
