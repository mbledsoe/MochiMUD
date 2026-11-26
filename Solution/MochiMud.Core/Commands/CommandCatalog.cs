
namespace MochiMud.Core.Commands
{
	public class CommandCatalog : ICommandCatalog
	{
		private List<string> catalog = [];

		public void AddEntry(string commandName)
		{
			catalog.Add(commandName);
		}

		public IReadOnlyList<string> GetAllCommandNames()
		{
			return catalog.OrderBy(c => c).ToList().AsReadOnly();
		}
	}
}
