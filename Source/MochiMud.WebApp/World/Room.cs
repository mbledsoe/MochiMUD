namespace MochiMud.WebApp.World
{
    public record Room(
        Guid Id,
        string Title,
        string Description,
        IReadOnlyCollection<Exit> Exits,
        IReadOnlyCollection<RoomObject> Objects)
    {
        public Room(Guid id, string title, string description, IReadOnlyCollection<Exit> exits)
            : this(id, title, description, exits, [])
        {
        }
    }

    public record Exit(Direction Direction, Guid DestinationRoomId);

    public record RoomObject(string Name, string Description, string? ReadText = null);

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
