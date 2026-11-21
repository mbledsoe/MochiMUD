using MochiMud.Core.Input;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.MainMenu;
using MochiMud.Core.World;

namespace MochiMud.Core.Commands
{
	public class QuitCommand : ICommand
	{
		public string CommandName => "quit";

		private readonly IPlayerOutput playerOutput;
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IWorkflowManager workflowManager;

		public QuitCommand(
			IPlayerOutput playerOutput,
			IActivePlayerManager activePlayerManager,
			IPlayerLocationManager playerLocationManager,
			IWorkflowManager workflowManager)
		{
			this.playerOutput = playerOutput;
			this.activePlayerManager = activePlayerManager;
			this.playerLocationManager = playerLocationManager;
			this.workflowManager = workflowManager;
		}

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			playerOutput.Send(player, "So long, and thanks for all the fish!");

			playerLocationManager.RemovePlayer(player);
			activePlayerManager.RemovePlayer(player.Id);

			workflowManager.StartWorkflow<MainMenuWorkflow>(player);
		}
	}
}
