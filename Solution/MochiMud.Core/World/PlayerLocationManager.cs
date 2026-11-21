using System.Collections.Concurrent;

namespace MochiMud.Core.World
{
	public class PlayerLocationManager : IPlayerLocationManager
	{
		private readonly ConcurrentDictionary<Guid, Guid> playerRooms = new();

		public void MovePlayerToRoom(ActivePlayer player, Guid roomId)
		{
			playerRooms.AddOrUpdate(player.Id, roomId, (playerId, roomId) => roomId);
		}

		public void RemovePlayer(ActivePlayer player)
		{
			playerRooms.TryRemove(player.Id, out var _);
		}
	}
}
