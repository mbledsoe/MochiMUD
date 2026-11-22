using System.Text;
using MochiMud.Core.World;

namespace MochiMud.Core.Output
{
	internal class HtmlOutputBuilder : IOutputBuilder
	{
		public string BuildRoomOutput(RoomData roomData)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"<div class=\"room-title\">{roomData.Title}</div>");
			sb.AppendLine($"<div class=\"room-desc\">{roomData.Description}</div>");

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
	}
}
