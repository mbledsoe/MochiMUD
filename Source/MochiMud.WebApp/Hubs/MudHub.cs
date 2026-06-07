using Microsoft.AspNetCore.SignalR;

namespace MochiMud.WebApp.Hubs
{
    public class MudHub : Hub
    {
        private readonly ILogger<MudHub> logger;

        public MudHub(ILogger<MudHub> logger)
        {
            this.logger = logger;
        }

        public Task SendCommand(string command)
        {
            logger.LogInformation("Received command from {ConnectionId}: {Command}", Context.ConnectionId, command);

            return Task.CompletedTask;
        }
    }
}
