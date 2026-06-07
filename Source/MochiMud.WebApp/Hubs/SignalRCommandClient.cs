using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;

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
    }
}
