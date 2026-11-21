namespace MochiMud.Core.World
{
	public interface IPlayerLocationManager
	{
		void MovePlayerToRoom(ActivePlayer player, Guid roomId);
		void RemovePlayer(ActivePlayer player);
	}
}