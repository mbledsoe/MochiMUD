using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MochiMud.WebApp.Storage
{
    public class DataDirectoryProvider
    {
        public DataDirectoryProvider(IOptions<DataStorageOptions> options, IHostEnvironment hostEnvironment)
        {
            var baseDataDirectory = options.Value.BaseDataDirectory;

            BaseDirectory = Path.IsPathRooted(baseDataDirectory)
                ? baseDataDirectory
                : Path.Combine(hostEnvironment.ContentRootPath, baseDataDirectory);
        }

        public string BaseDirectory { get; }

        public string GetDirectory(params string[] segments)
        {
            var directory = Path.Combine(new[] { BaseDirectory }.Concat(segments).ToArray());

            Directory.CreateDirectory(directory);

            return directory;
        }
    }
}
