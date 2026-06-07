namespace MochiMud.WebApp.Commands
{
    public class LookHandler : ICommandHandler
    {
        private readonly ILogger<LookHandler> logger;

        public LookHandler(ILogger<LookHandler> logger)
        {
            this.logger = logger;
        }

        public string CommandName => "look";

        public async Task HandleAsync(string command, ICommandClient client, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling look command: {Command}", command);

            await client.SendMessageAsync("hello world", cancellationToken);
        }
    }
}
