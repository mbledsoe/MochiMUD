using System.Collections.Concurrent;

namespace MochiMud.WebApp.Players
{
    public class PlayerConnectionRegistry
    {
        private readonly ConcurrentDictionary<string, Player> playersByConnectionId = new();

        public void AddOrUpdate(string connectionId, Player player)
        {
            playersByConnectionId[connectionId] = player;
        }

        public bool TryGetPlayer(string connectionId, out Player? player)
        {
            return playersByConnectionId.TryGetValue(connectionId, out player);
        }

        public bool Remove(string connectionId)
        {
            return playersByConnectionId.TryRemove(connectionId, out _);
        }
    }
}
