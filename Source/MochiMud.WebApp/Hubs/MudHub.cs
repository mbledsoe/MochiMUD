using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Hubs
{
    public class MudHub : Hub
    {
        private const string FakePlayerNamePrefix = "Test Player";

        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly IHubContext<MudHub> hubContext;
        private readonly ILogger<MudHub> logger;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;
        private readonly IPlayerDataService playerDataService;

        public MudHub(
            CommandExecutionQueue commandExecutionQueue,
            IHubContext<MudHub> hubContext,
            ILogger<MudHub> logger,
            PlayerConnectionRegistry playerConnectionRegistry,
            IPlayerDataService playerDataService)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.hubContext = hubContext;
            this.logger = logger;
            this.playerConnectionRegistry = playerConnectionRegistry;
            this.playerDataService = playerDataService;
        }

        public override async Task OnConnectedAsync()
        {
            var player = new Player($"{FakePlayerNamePrefix} {Random.Shared.Next(0, 10000):0000}");

            playerDataService.Add(player);
            playerConnectionRegistry.AddOrUpdate(Context.ConnectionId, player);

            logger.LogInformation(
                "Created player {PlayerName} for connection {ConnectionId}.",
                player.Name,
                Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (playerConnectionRegistry.TryRemovePlayer(Context.ConnectionId, out var player) && player is not null)
            {
                playerDataService.Remove(player);
            }

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

            var client = new SignalRCommandClient(hubContext, Context.ConnectionId);

            await commandExecutionQueue.EnqueueAsync(new QueuedCommand(command, client, player), Context.ConnectionAborted);
        }
    }
}
