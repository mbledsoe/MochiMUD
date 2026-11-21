using System.Text;
using MochiMud.Core.Workflows.Login;

namespace MochiMud.Core.Workflows.MainMenu
{
	public class MainMenuWorkflow : IWorkflow
	{
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IPlayerOutput playerOutput;
		private readonly IWorkflowManager workflowManager;
		private Guid activePlayerId;

		public MainMenuWorkflow(
			IActivePlayerManager activePlayerManager,
			IPlayerOutput playerOutput,
			IWorkflowManager workflowManager)
		{
			this.activePlayerManager = activePlayerManager;
			this.playerOutput = playerOutput;
			this.workflowManager = workflowManager;
		}

		public void Start(ActivePlayer player)
		{
			activePlayerId = player.Id;
			SendMenuOutput(player);
		}

		private void SendMenuOutput(ActivePlayer player)
		{
			var sb = new StringBuilder();
			sb.AppendLine("<div>Please make a selection</div>");
			sb.AppendLine("<div>1) Enter MochiMUD</div>");

			playerOutput.Send(player, sb.ToString());
		}

		public void HandleInput(string input)
		{
			switch (Convert.ToInt32(input))
			{
				case 1:
					HandleEnterMochiMud();
					break;
				default:
					break;
			}
		}

		private void HandleEnterMochiMud()
		{
			var activePlayer = activePlayerManager.GetById(activePlayerId)
				?? throw new MochiMudException("No active player!");

			workflowManager.CompleteWorkflow(activePlayer);
			workflowManager.StartWorkflow<LoginWorkflow>(activePlayer);
		}
	}
}
