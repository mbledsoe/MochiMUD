namespace MochiMud.WebApp.World
{
    public class StaticWorldDataService : IWorldDataService
    {
        private static readonly IReadOnlyDictionary<Guid, Room> Rooms = new Dictionary<Guid, Room>
        {
            [WorldConstants.DefaultStartRoomId] = new Room(
                WorldConstants.DefaultStartRoomId,
                "The Beginning",
                "You are standing in a quiet room. The air smells faintly of tea and old stone.")
        };

        public Task<Room?> GetRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            Rooms.TryGetValue(roomId, out var room);

            return Task.FromResult(room);
        }
    }
}
