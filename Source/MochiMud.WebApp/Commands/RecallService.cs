using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class RecallService
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly IHubContext<MudHub> hubContext;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;
        private readonly RoomPresenter roomPresenter;

        public RecallService(
            ICommandNotificationService commandNotificationService,
            IHubContext<MudHub> hubContext,
            PlayerConnectionRegistry playerConnectionRegistry,
            RoomPresenter roomPresenter)
        {
            this.commandNotificationService = commandNotificationService;
            this.hubContext = hubContext;
            this.playerConnectionRegistry = playerConnectionRegistry;
            this.roomPresenter = roomPresenter;
        }

        public async Task RecallAsync(
            Player player,
            ICommandClient? client,
            CancellationToken cancellationToken = default)
        {
            player.CurrentRoomId = WorldConstants.DefaultStartRoomId;

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                player.CurrentRoomId,
                player,
                $"A shimmering cloud suddenly appears and {player.Name} steps through!",
                cancellationToken);

            if (client is not null)
            {
                await client.SendMessageAsync("You recall to safety.", cancellationToken);
                await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);
                return;
            }

            await commandNotificationService.SendToPlayersAsync(
                [player],
                "You recall to safety.",
                cancellationToken);

            await SendRoomToConnectedClientsAsync(player, cancellationToken);
        }

        private async Task SendRoomToConnectedClientsAsync(Player player, CancellationToken cancellationToken)
        {
            foreach (var connectionId in playerConnectionRegistry.GetConnectionIdsForPlayers([player]))
            {
                var client = new SignalRCommandClient(hubContext, connectionId);
                await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);
            }
        }
    }
}
