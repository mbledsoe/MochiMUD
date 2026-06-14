namespace MochiMud.WebApp.Players
{
    public interface IPlayerDataService
    {
        void Add(Player player);

        IReadOnlyCollection<Player> GetPlayers();

        IReadOnlyCollection<Player> GetPlayersInRoom(Guid roomId);

        bool Remove(Player player);
    }
}
