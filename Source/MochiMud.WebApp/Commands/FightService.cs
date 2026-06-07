using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class FightService
    {
        private static readonly TimeSpan RoundDelay = TimeSpan.FromSeconds(2);

        private readonly ILogger<FightService> logger;
        private readonly List<ActiveFight> activeFights = new();

        public FightService(ILogger<FightService> logger)
        {
            this.logger = logger;
        }

        public void StartFight(Player player, Mob mob, ICommandClient client)
        {
            activeFights.Add(new ActiveFight(player, mob, client, DateTimeOffset.UtcNow));
        }

        public void StopFight(Player player)
        {
            activeFights.RemoveAll(activeFight => activeFight.Player.Id == player.Id);
            player.EndFight();
        }

        public async Task UpdateAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
        {
            foreach (var activeFight in activeFights.ToArray())
            {
                if (!activeFight.Player.IsInFight)
                {
                    activeFights.Remove(activeFight);
                    continue;
                }

                if (activeFight.Mob.HitPoints <= 0)
                {
                    await EndFightAsync(activeFight, cancellationToken);
                    continue;
                }

                if (activeFight.NextRoundAt > now)
                {
                    continue;
                }

                await ProcessRoundAsync(activeFight, now, cancellationToken);
            }
        }

        private async Task EndFightAsync(ActiveFight activeFight, CancellationToken cancellationToken)
        {
            try
            {
                await activeFight.Client.SendMessageAsync($"You killed the {activeFight.Mob.Name}!", cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to send fight completion message for player {PlayerId}.", activeFight.Player.Id);
            }
            finally
            {
                StopFight(activeFight.Player);
            }
        }

        private async Task ProcessRoundAsync(ActiveFight activeFight, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var damage = activeFight.Player.Weapon.DamageDice.Roll();
            activeFight.Mob.HitPoints -= damage;

            await activeFight.Client.SendMessageAsync(
                $"You attacked the {activeFight.Mob.Name} for {damage} hitpoints",
                cancellationToken);

            if (activeFight.Mob.HitPoints <= 0)
            {
                await EndFightAsync(activeFight, cancellationToken);
                return;
            }

            activeFight.NextRoundAt = now.Add(RoundDelay);
        }

        private class ActiveFight
        {
            public ActiveFight(Player player, Mob mob, ICommandClient client, DateTimeOffset nextRoundAt)
            {
                Player = player;
                Mob = mob;
                Client = client;
                NextRoundAt = nextRoundAt;
            }

            public Player Player { get; }

            public Mob Mob { get; }

            public ICommandClient Client { get; }

            public DateTimeOffset NextRoundAt { get; set; }
        }
    }
}
