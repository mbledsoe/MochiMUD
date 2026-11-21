using Microsoft.AspNetCore.SignalR;
using MochiMud.Core;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.Login;

namespace MochiMud.WebApp.Hubs
{
	public class MochiMudHub : Hub
	{
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IWorkflowManager workflowManager;

		public MochiMudHub(
			IActivePlayerManager activePlayerManager,
			IWorkflowManager workflowManager)
		{
			this.activePlayerManager = activePlayerManager;
			this.workflowManager = workflowManager;
		}

		public override async Task OnConnectedAsync()
		{
			await Clients.Caller.SendAsync("ReceiveMessage", Messages.Welcome);
			await base.OnConnectedAsync();

			var activePlayer = activePlayerManager.CreateActivePlayer(Context.ConnectionId);

			workflowManager.StartWorkflow<LoginWorkflow>(activePlayer);
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			var activePlayer = activePlayerManager.GetByConnectionId(Context.ConnectionId);

			if (activePlayer != null)
			{
				workflowManager.CancelWorkflow(activePlayer);
			}			

			return base.OnDisconnectedAsync(exception);
		}

		public async Task SendMessage(string message)
		{
			await Clients.Caller.SendAsync("ReceiveMessage", $"You said: {message}");			
		}
	}
}
