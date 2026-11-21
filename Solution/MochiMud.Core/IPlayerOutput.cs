namespace MochiMud.Core
{
	public interface IPlayerOutput
	{
		void Send(ActivePlayer player, string message);
	}
}