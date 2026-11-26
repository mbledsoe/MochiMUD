using System.Text;
using MochiMud.Core.World;

namespace MochiMud.Core.Output
{
	internal class HtmlOutputBuilder : IOutputBuilder
	{
		private readonly IRoomDataService roomDataService;

		public HtmlOutputBuilder(IRoomDataService roomDataService)
		{
			this.roomDataService = roomDataService;
		}

		public string BuildRoomOutput(RoomData roomData)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"<div class=\"room-title\">{roomData.Title}</div>");
			sb.AppendLine($"<div class=\"room-desc\">{roomData.Description}</div>");

			sb.AppendLine(BuildExitsOutput(roomData));

			return sb.ToString();
		}

		public string BuildWelcomeMessage()
		{
			return $"<pre>{Messages.Welcome}";
		}

		public string BuildMainMenuPrompt()
		{
			var sb = new StringBuilder();
			sb.AppendLine("<div>Please make a selection:</div>");
			sb.AppendLine("<div>1) Enter MochiMUD");

			return sb.ToString();
		}

		public string BuildExitsOutput(RoomData currentRoomData)
		{
			var sb = new StringBuilder();
			sb.AppendLine("<div class=\"exits\">");

			foreach (var exit in currentRoomData.Exits.OrderBy(e => (int)e.Direction))
			{
				var exitRoom = roomDataService.GetRoom(exit.RoomId);
				sb.AppendLine($"<div class=\"exit-direction\">[{exit.Direction.ToString()}] {exitRoom.Title}</div>");
			}

			sb.AppendLine("</div>");

			return sb.ToString();
		}

		public string BuildCommandList(IEnumerable<string> commands)
		{
			var sb = new StringBuilder();

			sb.Append("<div>The following commands are available:</div>");

			sb.Append("<div class=\"command-list\">");

			foreach (var command in commands)
			{
				sb.Append($"<div class=\"command-name\">{command}</div>");
			}

			sb.Append("</div>");

			return sb.ToString();
		}
	}
}
