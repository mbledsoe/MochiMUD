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
			yield return new(
				gameOptions.CurrentValue.StartingRoomId, 
				"The Waiting Room",
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus et ultricies ligula. Vivamus sed libero et odio condimentum ultricies sit amet semper mauris. Sed sollicitudin lacus vel purus sollicitudin aliquet. Aenean ut mi nec augue efficitur malesuada sed in enim. Vivamus nec nisi justo. Suspendisse imperdiet turpis tortor, at lobortis est pellentesque vel. Sed at tempus enim. Sed leo ante, dignissim nec dui at, gravida ornare risus. Maecenas nulla tortor, blandit ut tristique non, lobortis vehicula erat. Sed dignissim consequat facilisis. Aenean in velit imperdiet est sodales dapibus sed finibus metus. Fusce auctor ligula dolor, et imperdiet dui congue at. Mauris consectetur est sit amet leo ultrices condimentum. Sed quis libero felis. Proin egestas purus elit, vel commodo dui elementum in. Phasellus vitae commodo odio.\r\n\r\nCras sed orci volutpat, viverra felis ac, facilisis elit. Pellentesque sit amet ipsum dignissim risus congue vulputate in eget libero. Curabitur et dui nibh. Fusce sodales ultrices malesuada. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam sit amet sapien faucibus, rhoncus nibh eu, tristique ante. Nulla leo purus, viverra vel maximus nec, malesuada in turpis. Cras tempus augue quam.");
		}
	}
}
