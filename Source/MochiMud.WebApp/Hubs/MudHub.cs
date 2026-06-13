using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Connections;

namespace MochiMud.WebApp.Hubs
{
    [Authorize]
    public class MudHub : Hub
    {
        private readonly ConnectionManager connectionManager;
        private readonly IHubContext<MudHub> hubContext;
        private readonly ILogger<MudHub> logger;

        public MudHub(
            ConnectionManager connectionManager,
            IHubContext<MudHub> hubContext,
            ILogger<MudHub> logger)
        {
            this.connectionManager = connectionManager;
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            if (!TryGetAccountId(out var accountId))
            {
                logger.LogWarning("Connection {ConnectionId} has no account identifier.", Context.ConnectionId);
                Context.Abort();
                return;
            }

            await connectionManager.OnConnectedAsync(CreateConnection(accountId), Context.ConnectionAborted);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            TryGetAccountId(out var accountId);

            await connectionManager.OnDisconnectedAsync(CreateConnection(accountId), Context.ConnectionAborted);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(string command)
        {
            logger.LogInformation("Received command from {ConnectionId}: {Command}", Context.ConnectionId, command);

            if (!TryGetAccountId(out var accountId))
            {
                logger.LogWarning("Connection {ConnectionId} has no account identifier.", Context.ConnectionId);
                return;
            }

            await connectionManager.OnInputAsync(CreateConnection(accountId), command, Context.ConnectionAborted);
        }

        private IClientConnection CreateConnection(Guid accountId)
        {
            return new SignalRClientConnection(hubContext, Context.ConnectionId, accountId);
        }

        private bool TryGetAccountId(out Guid accountId)
        {
            var accountIdValue = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(accountIdValue, out accountId);
        }
    }
}
