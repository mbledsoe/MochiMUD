using MochiMud.Core.World;

namespace MochiMud.Core.Output
{
	public interface IOutputBuilder
	{
		string BuildWelcomeMessage();
		string BuildRoomOutput(RoomData roomData);
		string BuildMainMenuPrompt();
	}
}