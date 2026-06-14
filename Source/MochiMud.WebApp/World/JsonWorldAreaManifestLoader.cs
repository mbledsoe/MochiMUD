using System.Text.Json;

namespace MochiMud.WebApp.World
{
    public class JsonWorldAreaManifestLoader
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly IWorldFileStore worldFileStore;

        public JsonWorldAreaManifestLoader(IWorldFileStore worldFileStore)
        {
            this.worldFileStore = worldFileStore;
        }

        public async Task<IReadOnlyCollection<string>> LoadAreasAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            using var stream = await worldFileStore.OpenReadAsync(relativePath, cancellationToken);
            var areas = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<string>>(
                stream,
                SerializerOptions,
                cancellationToken);

            if (areas is null)
            {
                throw new InvalidOperationException($"World area manifest did not contain areas: {relativePath}");
            }

            return areas;
        }
    }
}
