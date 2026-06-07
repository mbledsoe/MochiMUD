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

        public IReadOnlyCollection<string> GetConnectionIdsForPlayers(IEnumerable<Player> players)
        {
            var playersSet = players.ToHashSet();

            return playersByConnectionId
                .Where(playerByConnectionId => playersSet.Contains(playerByConnectionId.Value))
                .Select(playerByConnectionId => playerByConnectionId.Key)
                .ToArray();
        }

        public bool TryRemovePlayer(string connectionId, out Player? player)
        {
            return playersByConnectionId.TryRemove(connectionId, out player);
        }

        public bool Remove(string connectionId)
        {
            return playersByConnectionId.TryRemove(connectionId, out _);
        }
    }
}
