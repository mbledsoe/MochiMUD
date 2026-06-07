namespace MochiMud.WebApp.Commands
{
    public interface ICommandClient
    {
        Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
    }
}
