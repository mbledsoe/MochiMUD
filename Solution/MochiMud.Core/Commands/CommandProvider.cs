
namespace MochiMud.Core.Commands
{
	public class CommandProvider : ICommandProvider
	{
		private Dictionary<string, ICommand> commands = new();

		public CommandProvider(IEnumerable<ICommand> commandList)
		{
			foreach (var command in commandList)
			{
				commands.Add(command.CommandName, command);
			}
		}

		public ICommand? GetCommandByName(string commandName)
		{
			if (string.IsNullOrWhiteSpace(commandName))
			{
				return null;
			}

			commands.TryGetValue(commandName.Trim().ToLower(), out var command);

			return command;
		}
	}
}
