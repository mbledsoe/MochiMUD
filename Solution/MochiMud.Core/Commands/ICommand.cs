using MochiMud.Core.Input;

namespace MochiMud.Core.Commands
{
	public interface ICommand
	{
		string CommandName { get; }

		void Execute(ActivePlayer player, MochiMudInput input);
	}
}