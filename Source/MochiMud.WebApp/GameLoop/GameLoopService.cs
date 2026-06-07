using MochiMud.WebApp.Commands;

namespace MochiMud.WebApp.GameLoop
{
    public class GameLoopService : BackgroundService
    {
        private static readonly TimeSpan LoopDelay = TimeSpan.FromMilliseconds(10);

        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly CommandExecutor commandExecutor;
        private readonly FightService fightService;
        private readonly ILogger<GameLoopService> logger;

        public GameLoopService(
            CommandExecutionQueue commandExecutionQueue,
            CommandExecutor commandExecutor,
            FightService fightService,
            ILogger<GameLoopService> logger)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.commandExecutor = commandExecutor;
            this.fightService = fightService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var queuedCommand in commandExecutionQueue.Drain())
                    {
                        try
                        {
                            await commandExecutor.ExecuteAsync(
                                queuedCommand.Command,
                                queuedCommand.Client,
                                queuedCommand.Player,
                                stoppingToken);
                        }
                        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                        {
                            throw;
                        }
                        catch (Exception exception)
                        {
                            logger.LogError(
                                exception,
                                "Failed to process command for player {PlayerId}.",
                                queuedCommand.Player.Id);
                        }
                    }

                    await fightService.UpdateAsync(DateTimeOffset.UtcNow, stoppingToken);
                    await Task.Delay(LoopDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Game loop iteration failed.");
                }
            }
        }
    }
}
