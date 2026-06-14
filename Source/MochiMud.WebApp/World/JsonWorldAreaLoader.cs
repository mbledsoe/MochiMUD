using System.Text.Json;
using System.Text.Json.Serialization;

namespace MochiMud.WebApp.World
{
    public class JsonWorldAreaLoader
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly IHostEnvironment environment;

        public JsonWorldAreaLoader(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        public IReadOnlyDictionary<Guid, Room> LoadRooms(string relativePath)
        {
            var path = Path.Combine(environment.ContentRootPath, relativePath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("World area data file was not found.", path);
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var rooms = JsonSerializer.Deserialize<IReadOnlyCollection<Room>>(stream, SerializerOptions);

            if (rooms is null)
            {
                throw new InvalidOperationException($"World area data file did not contain rooms: {path}");
            }

            return rooms.ToDictionary(room => room.Id);
        }
    }
}
