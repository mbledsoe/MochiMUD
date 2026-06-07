using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Hubs
{
    public class SignalRCommandClient : ICommandClient
    {
        private readonly IClientProxy clientProxy;

        public SignalRCommandClient(IClientProxy clientProxy)
        {
            this.clientProxy = clientProxy;
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await clientProxy.SendAsync("ReceiveMessage", message, cancellationToken);
        }

        public async Task SendRoomAsync(Room room, CancellationToken cancellationToken = default)
        {
            var message = $"{room.Title}{Environment.NewLine}{room.Description}";

            await SendMessageAsync(message, cancellationToken);
        }
    }
}
