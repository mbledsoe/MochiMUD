using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public record QueuedCommand(string Command, ICommandClient Client, Player Player);
}
