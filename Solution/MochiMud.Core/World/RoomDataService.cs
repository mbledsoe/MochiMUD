using Microsoft.Extensions.Options;
using MochiMud.Core.Config;

namespace MochiMud.Core.World
{
	public class RoomDataService : IRoomDataService
	{
		private readonly Dictionary<Guid, RoomData> allRooms = new();

		public void Initialize(IRoomDataStore roomDataStore)
		{
			foreach (var room in roomDataStore.GetRooms())
			{
				allRooms[room.Id] = room;
			}
		}

		public RoomData GetRoom(Guid roomId)
		{
			return allRooms[roomId];
		}
	}
}
