using Microsoft.AspNetCore.SignalR;
using MochiMud.Core;
using MochiMud.WebApp.Hubs;

namespace MochiMud.WebApp.Services
{
	public class SignalRPlayerOutput : IPlayerOutput
	{
		private readonly IHubContext<MochiMudHub> hubContext;

		public SignalRPlayerOutput(IHubContext<MochiMudHub> hubContext)
		{
			this.hubContext = hubContext;
		}

		public void Send(ActivePlayer player, string message)
		{
			hubContext.Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", message);
		}
	}
}
