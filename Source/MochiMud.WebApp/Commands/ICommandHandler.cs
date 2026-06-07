namespace MochiMud.WebApp.Commands
{
    public interface ICommandHandler
    {
        string CommandName { get; }

        Task HandleAsync(string command, ICommandClient client, CancellationToken cancellationToken = default);
    }
}
