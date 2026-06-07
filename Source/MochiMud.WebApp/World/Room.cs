namespace MochiMud.WebApp.World
{
    public record Room(Guid Id, string Title, string Description, IReadOnlyCollection<Exit> Exits);

    public record Exit(Direction Direction, Guid DestinationRoomId);

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
