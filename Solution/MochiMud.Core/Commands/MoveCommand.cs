using MochiMud.Core.Input;
using MochiMud.Core.Output;
using MochiMud.Core.World;

namespace MochiMud.Core.Commands
{
	public class MoveCommand : ICommand
	{
		private readonly IOutputBuilder outputBuilder;
		private readonly IOutputQueue outputQueue;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IRoomDataService roomDataService;
		private readonly Direction moveDirection;

		public MoveCommand(
			IOutputBuilder outputBuilder,
			IOutputQueue outputQueue,
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService,
			Direction moveDirection)
		{
			this.outputBuilder = outputBuilder;
			this.outputQueue = outputQueue;
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
			this.moveDirection = moveDirection;
		}

		public string CommandName => moveDirection.ToString().ToLower();

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			var currentRoomId = playerLocationManager.GetPlayerRoomId(player);
			var currentRoomData = roomDataService.GetRoom(currentRoomId);
			var exit = currentRoomData.Exits.SingleOrDefault(e => e.Direction == moveDirection);

			if (exit == null)
			{
				outputQueue.Enqueue(new MochiMudOutput(player.Id, "There is not exit in that direction."));
				return;
			}

			var nextRoomData = roomDataService.GetRoom(exit.RoomId);

			playerLocationManager.MovePlayerToRoom(player, exit.RoomId);
			outputQueue.Enqueue(new MochiMudOutput(player.Id, outputBuilder.BuildRoomOutput(nextRoomData)));
		}
	}
}
