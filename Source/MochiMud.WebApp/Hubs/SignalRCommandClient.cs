using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;
using System.Text;

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
            await hubContext.Clients
                .Client(connectionId)
                .SendAsync("ReceiveHtmlMessage", HtmlMessageFormatter.FormatTextMessage(message), cancellationToken);
        }

        public async Task SendBannerAsync(string banner, CancellationToken cancellationToken = default)
        {
            await hubContext.Clients
                .Client(connectionId)
                .SendAsync("ReceiveHtmlMessage", HtmlMessageFormatter.FormatBanner(banner), cancellationToken);
        }

        public async Task SendRoomAsync(
            Room room,
            IReadOnlyCollection<Mob> mobs,
            IReadOnlyCollection<Player> players,
            string? exitsText = null,
            CancellationToken cancellationToken = default)
        {
            var message = new StringBuilder();

            message.AppendLine("<div class=\"room\">");
            message.AppendLine($"<div class=\"room-title\">{HtmlMessageFormatter.HtmlEncode(room.Title)}</div>");
            message.AppendLine($"<div class=\"room-description\">{HtmlMessageFormatter.HtmlEncode(room.Description)}</div>");

            if (mobs.Count > 0)
            {
                message.AppendLine("<div class=\"room-section room-mobs\">");

                foreach (var mob in mobs)
                {
                    message.AppendLine($"<div class=\"room-mob\">The {HtmlMessageFormatter.HtmlEncode(mob.Name)} is standing here.</div>");
                }

                message.AppendLine("</div>");
            }

            if (room.Objects.Count > 0)
            {
                message.AppendLine("<div class=\"room-section room-objects\">");

                foreach (var roomObject in room.Objects)
                {
                    message.AppendLine($"<div class=\"room-object\">{HtmlMessageFormatter.HtmlEncode(roomObject.Description)}</div>");
                }

                message.AppendLine("</div>");
            }

            if (players.Count > 0)
            {
                message.AppendLine("<div class=\"room-section room-players\">");

                foreach (var player in players)
                {
                    message.AppendLine($"<div class=\"room-player\">{FormatPlayerRoomText(player)}</div>");
                }

                message.AppendLine("</div>");
            }

            if (!string.IsNullOrWhiteSpace(exitsText))
            {
                message.AppendLine("<div class=\"room-section room-exits\">");
                message.AppendLine($"<div class=\"room-exit\">{HtmlMessageFormatter.HtmlEncode(exitsText)}</div>");
                message.AppendLine("</div>");
            }

            message.AppendLine("</div>");

            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveHtmlMessage", message.ToString(), cancellationToken);
        }

        private static string FormatPlayerRoomText(Player player)
        {
            var playerName = HtmlMessageFormatter.HtmlEncode(player.Name);

            return player.State switch
            {
                CharacterState.Fighting => $"{playerName} is fighting for his life!",
                CharacterState.Dead => $"The corpse of {playerName} is laying on the ground",
                _ => $"{playerName} is standing here."
            };
        }
    }
}
