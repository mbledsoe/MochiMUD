namespace MochiMud.WebApp.World
{
    public class StaticWorldDataService : IWorldDataService
    {
        private static readonly IReadOnlyDictionary<Guid, Room> Rooms = CreateRooms();

        public Task<Room?> GetRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            Rooms.TryGetValue(roomId, out var room);

            return Task.FromResult(room);
        }

        private static IReadOnlyDictionary<Guid, Room> CreateRooms()
        {
            var rooms = new Dictionary<Guid, Room>();

            AddRooms(rooms, ChessboardWorldBuilder.CreateRooms());
            AddRooms(rooms, HockesdenWorldBuilder.CreateRooms());

            return rooms;
        }

        private static void AddRooms(Dictionary<Guid, Room> rooms, IReadOnlyDictionary<Guid, Room> areaRooms)
        {
            foreach (var room in areaRooms.Values)
            {
                if (!rooms.TryAdd(room.Id, room))
                {
                    throw new InvalidOperationException($"Duplicate room id registered: {room.Id}");
                }
            }
        }
    }
}
