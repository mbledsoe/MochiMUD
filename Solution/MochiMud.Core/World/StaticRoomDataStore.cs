namespace MochiMud.Core.World
{
	public class StaticRoomDataStore : IRoomDataStore
	{
		private static readonly Guid waitingRoomId = Guid.Parse("B3D665FB-CD35-4AE5-AACF-C873CA975485");
		private static readonly Guid behindTheCurtainsId = Guid.Parse("1AB74D2F-DC81-42B0-9819-210FEFBCE1CE");
		private static readonly Guid endOfTheHallId = Guid.Parse("3320268B-AE1A-4540-8533-0D593D81CDC7");

		public IList<RoomData> GetRooms()
		{
			return
			[
				new(waitingRoomId,
					"The Waiting Room",
					"The floor tiles create a stark white and black zig-zag pattern.  All four walls are covered in deep red curtains.  A shadow in the northern wall seems to indicate a break in the curtain abric.",
					[new Exit(Direction.North, behindTheCurtainsId)]),

				new(behindTheCurtainsId,
					"Behind the Curtain",
					"A narrow hallway extends towards a dead end where the pale silhouette of a statue can be seen against the curtains.  The deep red curtains extend from the floor to the ceiling, some twenty feet above you.  To the south, a slight break in the curtain hints at a larger room beyond this hall.",
					[
						new Exit(Direction.South, waitingRoomId),
						new Exit(Direction.West, endOfTheHallId)
					]),

				new(endOfTheHallId,
					"The End of the Hall",
					"The hall comes to an abrupt end here and the curtains close in around you.  A pale white replica of Venus de Milo stands here, her blank stare peering through you towards the opposite end of the hallway.",
					[new Exit(Direction.East, behindTheCurtainsId)])
			];
		}
	}
}
