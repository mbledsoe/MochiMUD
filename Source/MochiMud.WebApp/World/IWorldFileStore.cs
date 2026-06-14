namespace MochiMud.WebApp.World
{
    public interface IWorldFileStore
    {
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);
    }
}
