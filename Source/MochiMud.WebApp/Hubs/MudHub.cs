using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;

namespace MochiMud.WebApp.Hubs
{
    public class MudHub : Hub
    {
        private readonly CommandProcessor commandProcessor;
        private readonly ILogger<MudHub> logger;

        public MudHub(CommandProcessor commandProcessor, ILogger<MudHub> logger)
        {
            this.commandProcessor = commandProcessor;
            this.logger = logger;
        }

        public async Task SendCommand(string command)
        {
            logger.LogInformation("Received command from {ConnectionId}: {Command}", Context.ConnectionId, command);

            var client = new SignalRCommandClient(Clients.Caller);

            await commandProcessor.ProcessAsync(command, client, Context.ConnectionAborted);
        }
    }
}
