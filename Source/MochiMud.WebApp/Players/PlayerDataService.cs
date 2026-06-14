using System.Collections.Concurrent;

namespace MochiMud.WebApp.Players
{
    public class PlayerDataService : IPlayerDataService
    {
        private readonly ConcurrentDictionary<Guid, Player> playersById = new();

        public void Add(Player player)
        {
            playersById.TryAdd(player.Id, player);
        }

        public IReadOnlyCollection<Player> GetPlayers()
        {
            return playersById.Values.ToArray();
        }

        public IReadOnlyCollection<Player> GetPlayersInRoom(Guid roomId)
        {
            return playersById.Values
                .Where(player => player.CurrentRoomId == roomId)
                .ToArray();
        }

        public bool Remove(Player player)
        {
            return playersById.TryRemove(player.Id, out _);
        }
    }
}
