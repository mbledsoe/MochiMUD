using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;

namespace MochiMud.WebApp.Players
{
    public interface ILevelingService
    {
        Task AwardExperienceAsync(Player player, int amount, CancellationToken cancellationToken = default);

        Task AwardMobKillExperienceAsync(
            Player player,
            Mob mob,
            int participantCount,
            CancellationToken cancellationToken = default);
    }

    public sealed class LevelingService(
        ICommandNotificationService commandNotificationService,
        IPlayerStore playerStore) : ILevelingService
    {
        public async Task AwardExperienceAsync(Player player, int amount, CancellationToken cancellationToken = default)
        {
            var levelsGained = player.AddExperience(amount);

            if (levelsGained > 0)
            {
                await commandNotificationService.SendToPlayersAsync(
                    [player],
                    "You leveled up!",
                    cancellationToken);
            }
        }

        public async Task AwardMobKillExperienceAsync(
            Player player,
            Mob mob,
            int participantCount,
            CancellationToken cancellationToken = default)
        {
            var amount = ExperienceAwardCalculator.CalculateMobKillExperience(mob, player, participantCount);

            if (amount <= 0)
            {
                return;
            }

            await AwardExperienceAsync(player, amount, cancellationToken);

            await commandNotificationService.SendToPlayersAsync(
                [player],
                $"You gain {amount} experience!",
                cancellationToken);

            await playerStore.SaveAsync(player, cancellationToken);
        }
    }
}
