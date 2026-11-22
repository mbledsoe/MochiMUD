using Microsoft.AspNetCore.SignalR;
using MochiMud.Core;
using MochiMud.Core.Input;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.Login;
using MochiMud.Core.Workflows.MainMenu;

namespace MochiMud.WebApp.Hubs
{
	public class MochiMudHub : Hub
	{
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IWorkflowManager workflowManager;
		private readonly IInputQueue inputQueue;

		public MochiMudHub(
			IActivePlayerManager activePlayerManager,
			IWorkflowManager workflowManager,
			IInputQueue inputQueue)
		{
			this.activePlayerManager = activePlayerManager;
			this.workflowManager = workflowManager;
			this.inputQueue = inputQueue;
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			
			var activePlayer = activePlayerManager.CreateActivePlayer(Context.ConnectionId);
			workflowManager.StartWorkflow<MainMenuWorkflow>(activePlayer);
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			ActivePlayer? activePlayer = GetActivePlayer();

			if (activePlayer != null)
			{
				workflowManager.CancelWorkflow(activePlayer);
			}

			return base.OnDisconnectedAsync(exception);
		}

		public async Task SendMessage(string message)
		{
			var activePlayer = GetActivePlayer()
				?? throw new MochiMudException($"No active player!");

			var input = new MochiMudInput(activePlayer.Id, message);
			inputQueue.Enqueue(input);
		}

		private ActivePlayer? GetActivePlayer()
		{
			return activePlayerManager.GetByConnectionId(Context.ConnectionId);
		}
	}
}
