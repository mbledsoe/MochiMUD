using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class KillHandler : CommandHandlerBase
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly FightService fightService;
        private readonly ILogger<KillHandler> logger;
        private readonly IMobDataService mobDataService;

        public KillHandler(
            ICommandNotificationService commandNotificationService,
            FightService fightService,
            ILogger<KillHandler> logger,
            IMobDataService mobDataService)
        {
            this.commandNotificationService = commandNotificationService;
            this.fightService = fightService;
            this.logger = logger;
            this.mobDataService = mobDataService;
        }

        public override string CommandName => "kill";

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling kill command: {Command}", command);

            var mobName = CommandTextParser.GetArguments(command);

            if (mobName is null)
            {
                await client.SendMessageAsync("Kill what?", cancellationToken);
                return;
            }

            var mobs = await mobDataService.GetMobsInRoomAsync(player.CurrentRoomId, cancellationToken);
            var mob = mobs.FirstOrDefault(mob => string.Equals(mob.Name, mobName, StringComparison.OrdinalIgnoreCase));

            if (mob is null)
            {
                await client.SendMessageAsync("You don't see that here.", cancellationToken);
                return;
            }

            if (!player.TryStartFight())
            {
                await client.SendMessageAsync("You are already in a fight.", cancellationToken);
                return;
            }

            try
            {
                await commandNotificationService.SendToPlayersInRoomExceptAsync(
                    player.CurrentRoomId,
                    player,
                    $"{player.Name} attacked the {mob.Name}",
                    cancellationToken);

                fightService.StartFight(player, mob);
            }
            catch
            {
                player.EndFight();
                throw;
            }
        }
    }
}
