using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Connections;

namespace MochiMud.WebApp.Hubs
{
    public sealed class SignalRClientConnection : IClientConnection
    {
        public SignalRClientConnection(IHubContext<MudHub> hubContext, string connectionId, Guid accountId)
        {
            ConnectionId = connectionId;
            AccountId = accountId;
            Client = new SignalRCommandClient(hubContext, connectionId);
        }

        public string ConnectionId { get; }

        public Guid AccountId { get; }

        public ICommandClient Client { get; }
    }
}
