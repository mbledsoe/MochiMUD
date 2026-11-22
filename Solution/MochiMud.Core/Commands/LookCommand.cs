using System.Text;
using MochiMud.Core.Input;
using MochiMud.Core.Output;
using MochiMud.Core.World;

namespace MochiMud.Core.Commands
{
	public class LookCommand : ICommand
	{
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IRoomDataService roomDataService;
		private readonly IOutputQueue outputQueue;
		private readonly IOutputBuilder outputBuilder;

		public string CommandName => "look";

		public LookCommand(
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService,
			IOutputQueue outputQueue,
			IOutputBuilder outputBuilder)
		{
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
			this.outputQueue = outputQueue;
			this.outputBuilder = outputBuilder;
		}

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			var roomId = playerLocationManager.GetPlayerRoomId(player);
			var roomData = roomDataService.GetRoom(roomId);

			outputQueue.Enqueue(new MochiMudOutput(player.Id, outputBuilder.BuildRoomOutput(roomData)));
		}
	}
}
