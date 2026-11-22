using Microsoft.AspNetCore.SignalR;
using MochiMud.Core;
using MochiMud.Core.Output;
using MochiMud.WebApp.Hubs;

namespace MochiMud.WebApp.Services
{
	public class SignalROutputSender : IOutputSender
	{
		private readonly IHubContext<MochiMudHub> hubContext;
		private readonly IActivePlayerManager activePlayerManager;

		public SignalROutputSender(
			IHubContext<MochiMudHub> hubContext,
			IActivePlayerManager activePlayerManager)
		{
			this.hubContext = hubContext;
			this.activePlayerManager = activePlayerManager;
		}

		public void Send(MochiMudOutput output)
		{
			var activePlayer = activePlayerManager.GetById(output.ActivePlayerId);

			if (activePlayer == null)
			{
				return;
			}

			hubContext.Clients.Client(activePlayer.ConnectionId).SendAsync("ReceiveMessage", output.Message);
		}
	}
}
