namespace MochiMud.Core
{
	public interface IActivePlayerManager
	{
		public ActivePlayer CreateActivePlayer(string connectionId);
		ActivePlayer? GetByConnectionId(string connectionId);
		ActivePlayer? GetById(Guid activePlayerId);
		void RemovePlayer(Guid activePlayerId);
	}
}