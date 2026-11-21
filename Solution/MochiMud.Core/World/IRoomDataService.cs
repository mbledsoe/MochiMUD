namespace MochiMud.Core.World
{
	public interface IRoomDataService
	{
		RoomData GetRoom(Guid roomId);
	}
}