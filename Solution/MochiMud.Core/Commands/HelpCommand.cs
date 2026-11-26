using System.Text;
using MochiMud.Core.Input;
using MochiMud.Core.Output;

namespace MochiMud.Core.Commands
{
	public class HelpCommand : ICommand
	{
		private readonly ICommandCatalog commandCatalog;
		private readonly IOutputQueue outputQueue;
		private readonly IOutputBuilder outputBuilder;

		public string CommandName => "help";

		public HelpCommand(ICommandCatalog commandCatalog,
			IOutputQueue outputQueue,
			IOutputBuilder outputBuilder)
		{
			this.commandCatalog = commandCatalog;
			this.outputQueue = outputQueue;
			this.outputBuilder = outputBuilder;
		}

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			var commands = commandCatalog.GetAllCommandNames();

			var commandListOutput = outputBuilder.BuildCommandList(commands);

			outputQueue.Enqueue(new MochiMudOutput(player.Id, commandListOutput));
		}
	}
}
