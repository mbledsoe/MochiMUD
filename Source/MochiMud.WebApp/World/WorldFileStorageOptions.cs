namespace MochiMud.WebApp.World
{
    public class WorldFileStorageOptions
    {
        public const string SectionName = "WorldFileStorage";

        public WorldFileStorageProvider Provider { get; set; } = WorldFileStorageProvider.LocalFileSystem;

        public LocalWorldFileStorageOptions LocalFileSystem { get; set; } = new();

        public AzureBlobWorldFileStorageOptions AzureBlobStorage { get; set; } = new();
    }

    public class LocalWorldFileStorageOptions
    {
        public string BasePath { get; set; } = string.Empty;
    }

    public class AzureBlobWorldFileStorageOptions
    {
        public string ContainerName { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;

        public Uri? ServiceUri { get; set; }
    }
}
