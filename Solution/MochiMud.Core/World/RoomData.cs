namespace MochiMud.Core.World
{
	public record RoomData(		
		Guid Id,
		string Title,
		string Description,
		IList<Exit> Exits);
}
