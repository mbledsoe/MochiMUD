
namespace MochiMud.Core.Commands
{
	public class CommandProvider : ICommandProvider
	{
		private Dictionary<string, ICommand> commands = new();

		public CommandProvider(IEnumerable<ICommand> commandList,
			ICommandCatalog commandCatalog)
		{
			foreach (var command in commandList)
			{
				commands.Add(command.CommandName, command);
				commandCatalog.AddEntry(command.CommandName);
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
