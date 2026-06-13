using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class FightService
    {
        private static readonly TimeSpan RoundDelay = TimeSpan.FromSeconds(2);

        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<FightService> logger;
        private readonly List<ActiveFight> activeFights = new();
        private readonly PlayerGroupService playerGroupService;

        public FightService(
            ICommandNotificationService commandNotificationService,
            ILogger<FightService> logger,
            PlayerGroupService playerGroupService)
        {
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.playerGroupService = playerGroupService;
        }

        public void StartFight(Player attacker, Mob mob)
        {
            var participants = playerGroupService
                .GetGroupMembers(attacker)
                .Where(player => player.CurrentRoomId == attacker.CurrentRoomId)
                .Where(player =>
                    ReferenceEquals(player, attacker)
                        ? player.State == CharacterState.Fighting || player.TryStartFight()
                        : player.TryStartFight())
                .ToArray();

            if (participants.Length == 0)
            {
                return;
            }

            activeFights.Add(new ActiveFight(attacker, participants, mob, DateTimeOffset.UtcNow));
        }

        public void StopFight(Player player)
        {
            foreach (var activeFight in activeFights.ToArray())
            {
                if (!activeFight.RemoveParticipant(player))
                {
                    continue;
                }

                if (activeFight.Participants.Count == 0)
                {
                    activeFights.Remove(activeFight);
                }
            }

            if (player.State == CharacterState.Fighting)
            {
                player.State = CharacterState.Standing;
            }
        }

        public async Task UpdateAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
        {
            foreach (var activeFight in activeFights.ToArray())
            {
                var activePlayers = activeFight.GetActivePlayers();

                if (activePlayers.Count == 0)
                {
                    activeFights.Remove(activeFight);
                    continue;
                }

                if (activeFight.Mob.HitPoints <= 0)
                {
                    await EndFightWithMobDeathAsync(activeFight, cancellationToken);
                    continue;
                }

                if (activeFight.NextRoundAt > now)
                {
                    continue;
                }

                await ProcessRoundAsync(activeFight, now, cancellationToken);
            }
        }

        private async Task EndFightWithMobDeathAsync(ActiveFight activeFight, CancellationToken cancellationToken)
        {
            try
            {
                await commandNotificationService.SendToPlayersAsync(
                    activeFight.Participants,
                    $"You killed the {activeFight.Mob.Name}!",
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to send fight completion message for mob {MobName}.", activeFight.Mob.Name);
            }
            finally
            {
                activeFight.Mob.State = CharacterState.Dead;
                EndFight(activeFight);
            }
        }

        private void EndFightWithAllPlayersDead(ActiveFight activeFight)
        {
            activeFights.Remove(activeFight);
        }

        private async Task EndFightWithPlayerDeathAsync(ActiveFight activeFight, Player player, CancellationToken cancellationToken)
        {
            try
            {
                await commandNotificationService.SendToPlayersAsync(
                    [player],
                    $"The {activeFight.Mob.Name} killed you!",
                    cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to send player death message for player {PlayerId}.", player.Id);
            }
            finally
            {
                player.State = CharacterState.Dead;
                activeFight.RemoveParticipant(player);
            }
        }

        private async Task ProcessRoundAsync(ActiveFight activeFight, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var roundParticipants = activeFight.Participants.ToArray();

            try
            {
                foreach (var player in activeFight.GetActivePlayers())
                {
                    var playerDamage = Attack(player, activeFight.Mob);

                    await commandNotificationService.SendToPlayersAsync(
                        [player],
                        $"You attacked the {activeFight.Mob.Name} for {playerDamage} hitpoints",
                        cancellationToken);

                    if (activeFight.Mob.HitPoints <= 0)
                    {
                        await EndFightWithMobDeathAsync(activeFight, cancellationToken);
                        return;
                    }
                }

                var target = GetMobTarget(activeFight);
                var mobDamage = Attack(activeFight.Mob, target);

                await commandNotificationService.SendToPlayersAsync(
                    [target],
                    $"The {activeFight.Mob.Name} attacked you for {mobDamage} hitpoints",
                    cancellationToken);

                if (target.HitPoints <= 0)
                {
                    await EndFightWithPlayerDeathAsync(activeFight, target, cancellationToken);
                }

                if (activeFight.GetActivePlayers().Count == 0)
                {
                    EndFightWithAllPlayersDead(activeFight);
                    return;
                }

                activeFight.NextRoundAt = now.Add(RoundDelay);
            }
            finally
            {
                await SendPlayerStatsUpdatesAsync(roundParticipants, cancellationToken);
            }
        }

        private async Task SendPlayerStatsUpdatesAsync(
            IEnumerable<Player> players,
            CancellationToken cancellationToken)
        {
            foreach (var player in players)
            {
                try
                {
                    await commandNotificationService.SendPlayerStatsUpdateAsync(player, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Failed to send player stats update for player {PlayerId}.", player.Id);
                }
            }
        }

        private void EndFight(ActiveFight activeFight)
        {
            foreach (var player in activeFight.Participants)
            {
                if (player.State == CharacterState.Fighting)
                {
                    player.State = CharacterState.Standing;
                }
            }

            activeFights.Remove(activeFight);
        }

        private static int Attack(Character attacker, Character target)
        {
            var damage = attacker.Weapon?.DamageDice.Roll() ?? attacker.BareHandDamage.Roll();
            target.HitPoints -= damage;

            return damage;
        }

        private static Player GetMobTarget(ActiveFight activeFight)
        {
            var activePlayers = activeFight.GetActivePlayers();

            if (activePlayers.Contains(activeFight.CurrentTargetPlayer))
            {
                return activeFight.CurrentTargetPlayer;
            }

            activeFight.CurrentTargetPlayer = activePlayers.ElementAt(Random.Shared.Next(activePlayers.Count));

            return activeFight.CurrentTargetPlayer;
        }

        private class ActiveFight
        {
            private readonly List<Player> participants;

            public ActiveFight(Player initialTarget, IReadOnlyCollection<Player> participants, Mob mob, DateTimeOffset nextRoundAt)
            {
                CurrentTargetPlayer = initialTarget;
                this.participants = participants.ToList();
                Mob = mob;
                NextRoundAt = nextRoundAt;
            }

            public Player CurrentTargetPlayer { get; set; }

            public IReadOnlyCollection<Player> Participants => participants;

            public Mob Mob { get; }

            public DateTimeOffset NextRoundAt { get; set; }

            public IReadOnlyCollection<Player> GetActivePlayers()
            {
                return participants
                    .Where(player => player.State == CharacterState.Fighting && player.HitPoints > 0)
                    .ToArray();
            }

            public bool RemoveParticipant(Player player)
            {
                return participants.Remove(player);
            }
        }
    }
}
