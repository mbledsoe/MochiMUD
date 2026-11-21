namespace MochiMud.Core
{
	public interface IActivePlayerManager
	{
		public ActivePlayer CreateActivePlayer(string connectionId);
		ActivePlayer? GetByConnectionId(string connectionId);
	}
}