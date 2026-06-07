using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Hubs
{
    public class MudHub : Hub
    {
        private const string FakePlayerName = "Test Player";

        private readonly CommandProcessor commandProcessor;
        private readonly ILogger<MudHub> logger;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;

        public MudHub(
            CommandProcessor commandProcessor,
            ILogger<MudHub> logger,
            PlayerConnectionRegistry playerConnectionRegistry)
        {
            this.commandProcessor = commandProcessor;
            this.logger = logger;
            this.playerConnectionRegistry = playerConnectionRegistry;
        }

        public override async Task OnConnectedAsync()
        {
            var player = new Player(FakePlayerName);

            playerConnectionRegistry.AddOrUpdate(Context.ConnectionId, player);

            logger.LogInformation(
                "Created player {PlayerName} for connection {ConnectionId}.",
                player.Name,
                Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            playerConnectionRegistry.Remove(Context.ConnectionId);
            logger.LogInformation("Removed player for connection {ConnectionId}.", Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(string command)
        {
            logger.LogInformation("Received command from {ConnectionId}: {Command}", Context.ConnectionId, command);

            if (!playerConnectionRegistry.TryGetPlayer(Context.ConnectionId, out var player) || player is null)
            {
                logger.LogWarning("No player found for connection {ConnectionId}.", Context.ConnectionId);
                return;
            }

            var client = new SignalRCommandClient(Clients.Caller);

            await commandProcessor.ProcessAsync(command, client, player, Context.ConnectionAborted);
        }
    }
}
