namespace MochiMud.WebApp.World
{
    public interface IWorldDataService
    {
        Task<Room?> GetRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    }
}
