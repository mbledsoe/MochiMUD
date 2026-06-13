using MochiMud.WebApp.Commands;

namespace MochiMud.WebApp.Connections
{
    public interface IClientConnection
    {
        string ConnectionId { get; }

        Guid AccountId { get; }

        ICommandClient Client { get; }
    }
}
