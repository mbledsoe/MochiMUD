namespace MochiMud.Core.World
{
	public interface IPlayerLocationManager
	{
		Guid GetPlayerRoomId(ActivePlayer player);
		void MovePlayerToRoom(ActivePlayer player, Guid roomId);
		void RemovePlayer(ActivePlayer player);
	}
}