using System.Collections.Concurrent;

namespace MochiMud.Core.World
{
	public class PlayerLocationManager : IPlayerLocationManager
	{
		private readonly ConcurrentDictionary<Guid, Guid> playerRooms = new();

		public void MovePlayerToRoom(ActivePlayer player, Guid roomId)
		{
			playerRooms[player.Id] = roomId;
		}

		public void RemovePlayer(ActivePlayer player)
		{
			playerRooms.TryRemove(player.Id, out var _);
		}

		public Guid GetPlayerRoomId(ActivePlayer player)
		{
			return playerRooms[player.Id];
		}
	}
}
