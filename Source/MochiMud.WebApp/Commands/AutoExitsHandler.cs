using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class AutoExitsHandler : CommandHandlerBase
    {
        private readonly ILogger<AutoExitsHandler> logger;
        private readonly IPlayerStore playerStore;

        public AutoExitsHandler(ILogger<AutoExitsHandler> logger, IPlayerStore playerStore)
        {
            this.logger = logger;
            this.playerStore = playerStore;
        }

        public override bool CanExecuteInFight => true;

        public override string CommandName => "autoexits";

        public override async Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling autoexits command: {Command}", command);

            var arguments = CommandTextParser.GetArguments(command);
            var enabled = arguments is null
                ? !player.AutoExits
                : ParsePreference(arguments);

            if (enabled is null)
            {
                await client.SendMessageAsync("Use autoexits on or autoexits off.", cancellationToken);
                return;
            }

            player.AutoExits = enabled.Value;

            await playerStore.SaveAsync(player, cancellationToken);
            await client.SendMessageAsync($"AutoExits is now {(player.AutoExits ? "on" : "off")}.", cancellationToken);
        }

        private static bool? ParsePreference(string arguments)
        {
            return arguments.Trim().ToLowerInvariant() switch
            {
                "on" => true,
                "off" => false,
                _ => null,
            };
        }
    }
}
