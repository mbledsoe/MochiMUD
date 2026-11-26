using MochiMud.Core.Input;
using MochiMud.Core.Output;
using MochiMud.Core.World;

namespace MochiMud.Core.Commands
{
	public class ExitsCommand : ICommand
	{
		private readonly IOutputBuilder outputBuilder;
		private readonly IOutputQueue outputQueue;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IRoomDataService roomDataService;

		public ExitsCommand(
			IOutputBuilder outputBuilder,
			IOutputQueue outputQueue,
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService)
		{
			this.outputBuilder = outputBuilder;
			this.outputQueue = outputQueue;
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
		}

		public string CommandName => "exits";

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			var currentRoomId = playerLocationManager.GetPlayerRoomId(player);
			var currentRoomData = roomDataService.GetRoom(currentRoomId);

			outputQueue.Enqueue(new MochiMudOutput(player.Id, outputBuilder.BuildExitsOutput(currentRoomData)));
		}
	}
}
