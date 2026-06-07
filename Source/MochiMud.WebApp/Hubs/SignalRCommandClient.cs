using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Hubs
{
    public class SignalRCommandClient : ICommandClient
    {
        private readonly string connectionId;
        private readonly IHubContext<MudHub> hubContext;

        public SignalRCommandClient(IHubContext<MudHub> hubContext, string connectionId)
        {
            this.hubContext = hubContext;
            this.connectionId = connectionId;
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", message, cancellationToken);
        }

        public async Task SendRoomAsync(
            Room room,
            IReadOnlyCollection<Mob> mobs,
            IReadOnlyCollection<Player> players,
            CancellationToken cancellationToken = default)
        {
            var message = $"{room.Title}{Environment.NewLine}{room.Description}";

            if (mobs.Count > 0)
            {
                var mobText = string.Join(Environment.NewLine, mobs.Select(mob => $"The {mob.Name} is standing here."));
                message = $"{message}{Environment.NewLine}{mobText}";
            }

            if (players.Count > 0)
            {
                var playerText = string.Join(Environment.NewLine, players.Select(player => $"{player.Name} is standing here."));
                message = $"{message}{Environment.NewLine}{playerText}";
            }

            await SendMessageAsync(message, cancellationToken);
        }
    }
}
