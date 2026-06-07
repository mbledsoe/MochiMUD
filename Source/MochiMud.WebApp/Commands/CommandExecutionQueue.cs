namespace MochiMud.WebApp.Commands
{
    public class CommandExecutionQueue
    {
        private readonly SemaphoreSlim commandSemaphore = new(1, 1);

        public async Task ExecuteAsync(Func<Task> command, CancellationToken cancellationToken = default)
        {
            await commandSemaphore.WaitAsync(cancellationToken);

            try
            {
                await command();
            }
            finally
            {
                commandSemaphore.Release();
            }
        }
    }
}
