using Microsoft.Extensions.Options;

namespace MochiMud.WebApp.World
{
    public class LocalWorldFileStore : IWorldFileStore
    {
        private readonly string basePath;

        public LocalWorldFileStore(IHostEnvironment environment, IOptions<WorldFileStorageOptions> options)
        {
            var configuredBasePath = options.Value.LocalFileSystem.BasePath;

            basePath = string.IsNullOrWhiteSpace(configuredBasePath)
                ? environment.ContentRootPath
                : Path.IsPathRooted(configuredBasePath)
                    ? configuredBasePath
                    : Path.Combine(environment.ContentRootPath, configuredBasePath);
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            if (Path.IsPathRooted(path))
            {
                throw new ArgumentException("World file paths must be relative.", nameof(path));
            }

            var fullPath = Path.Combine(basePath, path);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("World data file was not found.", fullPath);
            }

            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Task.FromResult(stream);
        }
    }
}
