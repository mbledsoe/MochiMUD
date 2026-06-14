using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class ReviveHandler : CommandHandlerBase
    {
        private readonly ILogger<ReviveHandler> logger;

        public ReviveHandler(ILogger<ReviveHandler> logger)
        {
            this.logger = logger;
        }

        public override string CommandName => "revive";

        public override bool CanExecuteInFight => true;

        public override bool CanExecuteWhenDead => true;

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling revive command: {Command}", command);

            player.HitPoints = player.HitPoints.RestoreToMaximum();

            if (player.State == CharacterState.Dead)
            {
                player.State = CharacterState.Standing;
            }

            await client.SendMessageAsync("You have been revived.", cancellationToken);
        }
    }
}
