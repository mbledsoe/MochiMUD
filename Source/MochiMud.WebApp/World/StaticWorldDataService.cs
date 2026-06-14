namespace MochiMud.WebApp.World
{
    public class StaticWorldDataService : IWorldDataService
    {
        private const string AreasManifestPath = "World/Data/areas.json";

        private readonly IReadOnlyDictionary<Guid, Room> rooms;

        public StaticWorldDataService(
            JsonWorldAreaManifestLoader jsonWorldAreaManifestLoader,
            JsonWorldAreaLoader jsonWorldAreaLoader)
        {
            rooms = CreateRoomsAsync(jsonWorldAreaManifestLoader, jsonWorldAreaLoader)
                .GetAwaiter()
                .GetResult();
        }

        public Task<Room?> GetRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            rooms.TryGetValue(roomId, out var room);

            return Task.FromResult(room);
        }

        private static async Task<IReadOnlyDictionary<Guid, Room>> CreateRoomsAsync(
            JsonWorldAreaManifestLoader jsonWorldAreaManifestLoader,
            JsonWorldAreaLoader jsonWorldAreaLoader)
        {
            var rooms = new Dictionary<Guid, Room>();

            foreach (var areaPath in await jsonWorldAreaManifestLoader.LoadAreasAsync(AreasManifestPath))
            {
                AddRooms(rooms, await jsonWorldAreaLoader.LoadRoomsAsync(areaPath));
            }

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
