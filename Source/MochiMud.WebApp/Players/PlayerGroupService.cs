using System.Collections.Concurrent;

namespace MochiMud.WebApp.Players
{
    public class PlayerGroupService
    {
        private readonly ConcurrentDictionary<Guid, PlayerGroup> groupsByLeaderId = new();

        public PlayerGroup CreateGroup(Player leader, IEnumerable<Player> followers)
        {
            RemovePlayerFromFollowerLists(leader);

            var distinctFollowers = followers
                .Where(follower => !ReferenceEquals(follower, leader))
                .Distinct()
                .ToArray();

            foreach (var follower in distinctFollowers)
            {
                RemovePlayerFromFollowerLists(follower);
            }

            var group = new PlayerGroup(leader, distinctFollowers);
            groupsByLeaderId[leader.Id] = group;

            return group;
        }

        public void AddFollower(Player leader, Player follower)
        {
            if (ReferenceEquals(leader, follower))
            {
                return;
            }

            RemovePlayerFromFollowerLists(follower);

            var group = groupsByLeaderId.GetOrAdd(leader.Id, _ => new PlayerGroup(leader, []));

            group.AddFollower(follower);
        }

        public IReadOnlyCollection<Player> GetFollowers(Player leader)
        {
            return groupsByLeaderId.TryGetValue(leader.Id, out var group)
                ? group.Followers
                : [];
        }

        public IReadOnlyCollection<Player> GetGroupMembers(Player player)
        {
            if (groupsByLeaderId.TryGetValue(player.Id, out var leaderGroup))
            {
                return [leaderGroup.Leader, .. leaderGroup.Followers];
            }

            var followerGroup = groupsByLeaderId.Values.FirstOrDefault(group => group.Followers.Contains(player));

            return followerGroup is null
                ? [player]
                : [followerGroup.Leader, .. followerGroup.Followers];
        }

        public void RemovePlayer(Player player)
        {
            groupsByLeaderId.TryRemove(player.Id, out _);
            RemovePlayerFromFollowerLists(player);
        }

        private void RemovePlayerFromFollowerLists(Player player)
        {
            foreach (var group in groupsByLeaderId.Values)
            {
                group.RemoveFollower(player);
            }
        }
    }
}
