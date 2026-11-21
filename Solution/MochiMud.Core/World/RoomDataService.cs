using Microsoft.Extensions.Options;
using MochiMud.Core.Config;

namespace MochiMud.Core.World
{
	public class RoomDataService : IRoomDataService
	{
		private readonly IOptionsMonitor<GameOptions> gameOptions;
		private Dictionary<Guid, RoomData> allRooms = new();

		public RoomDataService(IOptionsMonitor<GameOptions> gameOptions)
		{
			this.gameOptions = gameOptions;

			foreach (var room in BuildRooms())
			{
				allRooms.Add(room.Id, room);
			}			
		}

		public RoomData GetRoom(Guid roomId)
		{
			return allRooms[roomId];
		}

		private IEnumerable<RoomData> BuildRooms()
		{
			yield return new(gameOptions.CurrentValue.StartingRoomId, "The Waiting Room", "You are sitting in a room.");
		}
	}
}
