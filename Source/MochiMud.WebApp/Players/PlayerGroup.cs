namespace MochiMud.WebApp.Players
{
    public class PlayerGroup
    {
        private readonly List<Player> followers = new();

        public PlayerGroup(Player leader, IEnumerable<Player> followers)
        {
            Leader = leader;
            this.followers.AddRange(followers.Where(follower => !ReferenceEquals(follower, leader)));
        }

        public Player Leader { get; }

        public IReadOnlyCollection<Player> Followers => followers;

        public void AddFollower(Player follower)
        {
            if (ReferenceEquals(follower, Leader) || followers.Contains(follower))
            {
                return;
            }

            followers.Add(follower);
        }

        public bool RemoveFollower(Player follower)
        {
            return followers.Remove(follower);
        }
    }
}
