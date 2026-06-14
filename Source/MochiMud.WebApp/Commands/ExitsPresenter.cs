using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class ExitsPresenter
    {
        private readonly IWorldDataService worldDataService;

        public ExitsPresenter(IWorldDataService worldDataService)
        {
            this.worldDataService = worldDataService;
        }

        public async Task<string> FormatExitsAsync(Room room, CancellationToken cancellationToken = default)
        {
            if (room.Exits.Count == 0)
            {
                return "You see no obvious exits.";
            }

            var exitDescriptions = new List<string>();

            foreach (var exit in room.Exits.OrderBy(exit => exit.Direction))
            {
                var destination = await worldDataService.GetRoomAsync(exit.DestinationRoomId, cancellationToken);
                var destinationName = destination?.Title ?? "Unknown";

                exitDescriptions.Add($"{exit.Direction}: {destinationName}");
            }

            return string.Join(Environment.NewLine, exitDescriptions);
        }
    }
}
