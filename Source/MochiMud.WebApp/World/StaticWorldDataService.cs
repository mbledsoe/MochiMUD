namespace MochiMud.WebApp.World
{
    public class StaticWorldDataService : IWorldDataService
    {
        private static readonly IReadOnlyDictionary<Guid, Room> Rooms = ChessboardWorldBuilder.CreateRooms();

        public Task<Room?> GetRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            Rooms.TryGetValue(roomId, out var room);

            return Task.FromResult(room);
        }
    }
}
