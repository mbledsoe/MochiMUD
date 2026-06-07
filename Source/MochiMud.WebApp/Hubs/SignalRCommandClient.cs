using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Hubs
{
    public class SignalRCommandClient : ICommandClient
    {
        private readonly IClientProxy clientProxy;

        public SignalRCommandClient(IClientProxy clientProxy)
        {
            this.clientProxy = clientProxy;
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await clientProxy.SendAsync("ReceiveMessage", message, cancellationToken);
        }

        public async Task SendRoomAsync(Room room, IReadOnlyCollection<Mob> mobs, CancellationToken cancellationToken = default)
        {
            var message = $"{room.Title}{Environment.NewLine}{room.Description}";

            if (mobs.Count > 0)
            {
                var mobText = string.Join(Environment.NewLine, mobs.Select(mob => $"The {mob.Name} is standing here."));
                message = $"{message}{Environment.NewLine}{mobText}";
            }

            await SendMessageAsync(message, cancellationToken);
        }
    }
}
