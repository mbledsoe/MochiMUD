namespace MochiMud.Core.Commands
{
	public interface ICommandCatalog
	{
		void AddEntry(string commandName);
		IReadOnlyList<string> GetAllCommandNames();
	}
}