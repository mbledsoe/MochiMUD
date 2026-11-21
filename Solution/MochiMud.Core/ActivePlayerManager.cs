using System.Collections.Concurrent;

namespace MochiMud.Core
{
	public class ActivePlayerManager : IActivePlayerManager
	{
		private ConcurrentDictionary<Guid, ActivePlayer> activePlayers = new();

		public ActivePlayer CreateActivePlayer(string connectionId)
		{
			var activePlayer = new ActivePlayer(connectionId);
			activePlayers.TryAdd(activePlayer.Id, activePlayer);

			return activePlayer;
		}

		public ActivePlayer? GetByConnectionId(string connectionId)
		{
			return activePlayers.Values.FirstOrDefault(ap => ap.ConnectionId == connectionId);			
		}
	}
}
