using MochiMud.Core.Input;
using MochiMud.Core.Output;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.MainMenu;
using MochiMud.Core.World;

namespace MochiMud.Core.Commands
{
	public class QuitCommand : ICommand
	{
		public string CommandName => "quit";

		private readonly IOutputQueue outputQueue;
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IWorkflowManager workflowManager;

		public QuitCommand(
			IOutputQueue outputQueue,
			IActivePlayerManager activePlayerManager,
			IPlayerLocationManager playerLocationManager,
			IWorkflowManager workflowManager)
		{
			this.outputQueue = outputQueue;
			this.activePlayerManager = activePlayerManager;
			this.playerLocationManager = playerLocationManager;
			this.workflowManager = workflowManager;
		}

		public void Execute(ActivePlayer player, MochiMudInput input)
		{
			outputQueue.Enqueue(new MochiMudOutput(player.Id, "So long, and thanks for all the fish!"));

			playerLocationManager.RemovePlayer(player);
			activePlayerManager.RemovePlayer(player.Id);

			workflowManager.StartWorkflow<MainMenuWorkflow>(player);
		}
	}
}
