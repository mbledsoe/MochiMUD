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

		public string CommandName => "look";

		public LookCommand(
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService,
			IOutputQueue outputQueue)
		{
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
			this.outputQueue = outputQueue;
		}

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			var roomId = playerLocationManager.GetPlayerRoomId(player);
			var roomData = roomDataService.GetRoom(roomId);

			var sb = new StringBuilder();
			sb.AppendLine($"<div>{roomData.Title}</div>");
			sb.AppendLine($"<div>{roomData.Description}</div>");

			outputQueue.Enqueue(new MochiMudOutput(player.Id, sb.ToString()));
		}
	}
}
