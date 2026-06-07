namespace MochiMud.WebApp.World
{
    public record Room(Guid Id, string Title, string Description, IReadOnlyCollection<Exit> Exits);

    public record Exit(string Direction, Guid DestinationRoomId);
}
