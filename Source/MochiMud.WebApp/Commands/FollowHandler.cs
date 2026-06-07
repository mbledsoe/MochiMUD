using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class FollowHandler : CommandHandlerBase
    {
        private readonly ILogger<FollowHandler> logger;
        private readonly IPlayerDataService playerDataService;
        private readonly PlayerGroupService playerGroupService;

        public FollowHandler(
            ILogger<FollowHandler> logger,
            IPlayerDataService playerDataService,
            PlayerGroupService playerGroupService)
        {
            this.logger = logger;
            this.playerDataService = playerDataService;
            this.playerGroupService = playerGroupService;
        }

        public override string CommandName => "follow";

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling follow command: {Command}", command);

            var leaderName = CommandTextParser.GetArguments(command);

            if (leaderName is null)
            {
                await client.SendMessageAsync("Follow whom?", cancellationToken);
                return;
            }

            var leader = playerDataService
                .GetPlayersInRoom(player.CurrentRoomId)
                .FirstOrDefault(roomPlayer =>
                    !ReferenceEquals(roomPlayer, player)
                    && string.Equals(roomPlayer.Name, leaderName, StringComparison.OrdinalIgnoreCase));

            if (leader is null)
            {
                await client.SendMessageAsync("You don't see that player here.", cancellationToken);
                return;
            }

            playerGroupService.AddFollower(leader, player);

            await client.SendMessageAsync($"You are now following {leader.Name}.", cancellationToken);
        }
    }
}
