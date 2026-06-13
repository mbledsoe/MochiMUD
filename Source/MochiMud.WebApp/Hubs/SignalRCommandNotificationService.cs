using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Hubs
{
    public class SignalRCommandNotificationService : ICommandNotificationService
    {
        private readonly IHubContext<MudHub> hubContext;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;
        private readonly IPlayerDataService playerDataService;

        public SignalRCommandNotificationService(
            IHubContext<MudHub> hubContext,
            PlayerConnectionRegistry playerConnectionRegistry,
            IPlayerDataService playerDataService)
        {
            this.hubContext = hubContext;
            this.playerConnectionRegistry = playerConnectionRegistry;
            this.playerDataService = playerDataService;
        }

        public async Task SendToPlayersAsync(
            IEnumerable<Player> players,
            string message,
            CancellationToken cancellationToken = default)
        {
            var connectionIds = playerConnectionRegistry.GetConnectionIdsForPlayers(players);

            if (connectionIds.Count == 0)
            {
                return;
            }

            await hubContext.Clients
                .Clients(connectionIds)
                .SendAsync("ReceiveHtmlMessage", HtmlMessageFormatter.FormatTextMessage(message), cancellationToken);
        }

        public async Task SendToPlayersInRoomExceptAsync(
            Guid roomId,
            Player excludedPlayer,
            string message,
            CancellationToken cancellationToken = default)
        {
            var players = playerDataService
                .GetPlayersInRoom(roomId)
                .Where(player => !ReferenceEquals(player, excludedPlayer));
            await SendToPlayersAsync(players, message, cancellationToken);
        }

        public async Task SendPlayerStatsUpdateAsync(
            Player player,
            CancellationToken cancellationToken = default)
        {
            var connectionIds = playerConnectionRegistry.GetConnectionIdsForPlayers([player]);

            if (connectionIds.Count == 0)
            {
                return;
            }

            var update = new PlayerStatsUpdate(player.Id, player.HitPoints, player.MaximumHitPoints);

            await hubContext.Clients
                .Clients(connectionIds)
                .SendAsync("ReceivePlayerStatsUpdate", update, cancellationToken);
        }
    }
}
