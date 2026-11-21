namespace MochiMud.Core.Commands
{
	public interface ICommandProvider
	{
		ICommand? GetCommandByName(string commandName);
	}
}