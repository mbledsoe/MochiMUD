using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class FleeHandler : CommandHandlerBase
    {
        private readonly FightService fightService;
        private readonly ILogger<FleeHandler> logger;
        private readonly MoveService moveService;
        private readonly IWorldDataService worldDataService;

        public FleeHandler(
            FightService fightService,
            ILogger<FleeHandler> logger,
            MoveService moveService,
            IWorldDataService worldDataService)
        {
            this.fightService = fightService;
            this.logger = logger;
            this.moveService = moveService;
            this.worldDataService = worldDataService;
        }

        public override bool CanExecuteInFight => true;

        public override string CommandName => "flee";

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling flee command: {Command}", command);

            if (!player.IsInFight)
            {
                await client.SendMessageAsync("But you're not fighting anyone!", cancellationToken);
                return;
            }

            var currentRoom = await worldDataService.GetRoomAsync(player.CurrentRoomId, cancellationToken);

            if (currentRoom is null)
            {
                logger.LogWarning("Room not found: {RoomId}", player.CurrentRoomId);
                await client.SendMessageAsync("You cannot flee from here.", cancellationToken);
                return;
            }

            var validExits = new List<Exit>();

            foreach (var exit in currentRoom.Exits)
            {
                var destinationRoom = await worldDataService.GetRoomAsync(exit.DestinationRoomId, cancellationToken);

                if (destinationRoom is not null)
                {
                    validExits.Add(exit);
                }
            }

            if (validExits.Count == 0)
            {
                await client.SendMessageAsync("You cannot flee from here.", cancellationToken);
                return;
            }

            var selectedExit = validExits[Random.Shared.Next(validExits.Count)];

            fightService.StopFight(player);
            await client.SendMessageAsync($"You fled {selectedExit.Direction}.", cancellationToken);
            await moveService.MoveAsync(selectedExit.Direction, client, player, cancellationToken);
        }
    }
}
