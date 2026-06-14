using System.Text.Json;

namespace MochiMud.WebApp.World
{
    public class JsonWorldAreaManifestLoader
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly IHostEnvironment environment;

        public JsonWorldAreaManifestLoader(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        public IReadOnlyCollection<string> LoadAreas(string relativePath)
        {
            var path = Path.Combine(environment.ContentRootPath, relativePath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("World area manifest file was not found.", path);
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var areas = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(stream, SerializerOptions);

            if (areas is null)
            {
                throw new InvalidOperationException($"World area manifest did not contain areas: {path}");
            }

            return areas;
        }
    }
}
