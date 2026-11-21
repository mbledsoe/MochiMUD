using Microsoft.AspNetCore.SignalR;

namespace MochiMud.WebApp.Hubs
{
	public class MochiMudHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await Clients.Caller.SendAsync("ReceiveMessage", Messages.Welcome);
			await base.OnConnectedAsync();
		}
	}
}
