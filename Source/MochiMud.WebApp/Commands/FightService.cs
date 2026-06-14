using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.Spells;

namespace MochiMud.WebApp.Commands
{
    public class FightService
    {
        private static readonly TimeSpan RoundDelay = TimeSpan.FromSeconds(2);

        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<FightService> logger;
        private readonly List<ActiveFight> activeFights = new();
        private readonly IMobDataService mobDataService;
        private readonly IPlayerDataService playerDataService;
        private readonly PlayerGroupService playerGroupService;

        public FightService(
            ICommandNotificationService commandNotificationService,
            ILogger<FightService> logger,
            IMobDataService mobDataService,
            IPlayerDataService playerDataService,
            PlayerGroupService playerGroupService)
        {
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.mobDataService = mobDataService;
            this.playerDataService = playerDataService;
            this.playerGroupService = playerGroupService;
        }

        public bool StartFight(Player attacker, Mob mob)
        {
            var activeFight = GetActiveFightForMob(mob);
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
                return false;
            }

            if (activeFight is not null)
            {
                activeFight.AddParticipants(participants);
                return true;
            }

            if (!mob.TryStartFight())
            {
                foreach (var participant in participants)
                {
                    participant.EndFight();
                }

                return false;
            }

            activeFights.Add(new ActiveFight(attacker, participants, mob, DateTimeOffset.UtcNow));
            return true;
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
                    EndFight(activeFight);
                }
            }

            if (player.State == CharacterState.Fighting)
            {
                player.State = CharacterState.Standing;
            }
        }

        public async Task CastDamageSpellAsync(
            Player caster,
            Mob target,
            SpellDefinition spell,
            CancellationToken cancellationToken = default)
        {
            var activeFight = GetActiveFightForPlayer(caster);

            if (activeFight is not null && !ReferenceEquals(activeFight.Mob, target))
            {
                await commandNotificationService.SendToPlayersAsync(
                    [caster],
                    $"You are already fighting the {activeFight.Mob.Name}.",
                    cancellationToken);
                return;
            }

            if (activeFight is null)
            {
                StartFight(caster, target);
                activeFight = GetActiveFightForPlayer(caster);
            }

            if (activeFight is null)
            {
                await commandNotificationService.SendToPlayersAsync(
                    [caster],
                    "You cannot cast that right now.",
                    cancellationToken);
                return;
            }

            caster.Mana = caster.Mana.Spend(spell.ManaCost);

            var damage = spell.EffectDice.Roll();
            activeFight.Mob.HitPoints = activeFight.Mob.HitPoints.Reduce(damage);

            await commandNotificationService.SendToPlayersAsync(
                [caster],
                $"You cast {spell.Name} at the {activeFight.Mob.Name} for {damage} hitpoints.",
                cancellationToken);

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                caster.CurrentRoomId,
                caster,
                $"{caster.Name} cast {spell.Name} at the {activeFight.Mob.Name}.",
                cancellationToken);

            await commandNotificationService.SendPlayerStatsUpdateAsync(caster, cancellationToken);

            if (activeFight.Mob.HitPoints.Current <= 0)
            {
                await EndFightWithMobDeathAsync(activeFight, cancellationToken);
            }
        }

        public async Task CastHealingSpellAsync(
            Player caster,
            Character target,
            SpellDefinition spell,
            CancellationToken cancellationToken = default)
        {
            caster.Mana = caster.Mana.Spend(spell.ManaCost);

            var healing = spell.EffectDice.Roll();
            var beforeHealing = target.HitPoints.Current;
            target.HitPoints = target.HitPoints.Restore(healing);
            var restoredHitPoints = target.HitPoints.Current - beforeHealing;

            await commandNotificationService.SendToPlayersAsync(
                [caster],
                $"You cast {spell.Name} on {target.Name} and restore {restoredHitPoints} hitpoints.",
                cancellationToken);

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                caster.CurrentRoomId,
                caster,
                $"{caster.Name} cast {spell.Name} on {target.Name}.",
                cancellationToken);

            await commandNotificationService.SendPlayerStatsUpdateAsync(caster, cancellationToken);

            if (target is Player targetPlayer && !ReferenceEquals(targetPlayer, caster))
            {
                await commandNotificationService.SendPlayerStatsUpdateAsync(targetPlayer, cancellationToken);
            }
        }

        public async Task UpdateAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
        {
            await StartAggressiveFightsAsync(cancellationToken);

            foreach (var activeFight in activeFights.ToArray())
            {
                var activePlayers = activeFight.GetActivePlayers();

                if (activePlayers.Count == 0)
                {
                    activeFights.Remove(activeFight);
                    continue;
                }

                if (activeFight.Mob.HitPoints.Current <= 0)
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

        private async Task StartAggressiveFightsAsync(CancellationToken cancellationToken)
        {
            var mobs = await mobDataService.GetMobsAsync(cancellationToken);

            foreach (var mob in mobs)
            {
                if (!CanStartAggressiveFight(mob))
                {
                    continue;
                }

                var targets = playerDataService
                    .GetPlayersInRoom(mob.CurrentRoomId)
                    .Where(player => player.State == CharacterState.Standing && player.HitPoints.Current > 0)
                    .ToArray();

                if (targets.Length == 0)
                {
                    continue;
                }

                var target = targets[Random.Shared.Next(targets.Length)];

                if (!StartFight(target, mob))
                {
                    continue;
                }

                await commandNotificationService.SendToPlayersAsync(
                    [target],
                    $"The {mob.Name} attacks you!",
                    cancellationToken);

                await commandNotificationService.SendToPlayersInRoomExceptAsync(
                    mob.CurrentRoomId,
                    target,
                    $"The {mob.Name} attacks {target.Name}!",
                    cancellationToken);
            }
        }

        private bool CanStartAggressiveFight(Mob mob)
        {
            return mob.IsAggressive
                && mob.State == CharacterState.Standing
                && mob.HitPoints.Current > 0
                && GetActiveFightForMob(mob) is null;
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
            EndFight(activeFight);
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

                    if (activeFight.Mob.HitPoints.Current <= 0)
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

                if (target.HitPoints.Current <= 0)
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

        private ActiveFight? GetActiveFightForPlayer(Player player)
        {
            return activeFights.FirstOrDefault(activeFight => activeFight.Participants.Contains(player));
        }

        private ActiveFight? GetActiveFightForMob(Mob mob)
        {
            return activeFights.FirstOrDefault(activeFight => ReferenceEquals(activeFight.Mob, mob));
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

            if (activeFight.Mob.State == CharacterState.Fighting)
            {
                activeFight.Mob.State = CharacterState.Standing;
            }

            activeFights.Remove(activeFight);
        }

        private static int Attack(Character attacker, Character target)
        {
            var damage = attacker.Weapon?.DamageDice.Roll() ?? attacker.BareHandDamage.Roll();
            target.HitPoints = target.HitPoints.Reduce(damage);

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
                    .Where(player => player.State == CharacterState.Fighting && player.HitPoints.Current > 0)
                    .ToArray();
            }

            public bool RemoveParticipant(Player player)
            {
                return participants.Remove(player);
            }

            public void AddParticipants(IEnumerable<Player> players)
            {
                foreach (var player in players)
                {
                    if (!participants.Contains(player))
                    {
                        participants.Add(player);
                    }
                }
            }
        }
    }
}
