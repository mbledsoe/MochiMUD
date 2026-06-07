using System.Collections.Concurrent;

namespace MochiMud.WebApp.Commands
{
    public class CommandExecutionQueue
    {
        private readonly ConcurrentQueue<QueuedCommand> queuedCommands = new();

        public Task EnqueueAsync(QueuedCommand queuedCommand, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            queuedCommands.Enqueue(queuedCommand);

            return Task.CompletedTask;
        }

        public IReadOnlyCollection<QueuedCommand> Drain()
        {
            var commandsToDrain = queuedCommands.Count;
            var drainedCommands = new List<QueuedCommand>(commandsToDrain);

            for (var i = 0; i < commandsToDrain; i++)
            {
                if (!queuedCommands.TryDequeue(out var queuedCommand))
                {
                    break;
                }

                drainedCommands.Add(queuedCommand);
            }

            return drainedCommands;
        }
    }
}
