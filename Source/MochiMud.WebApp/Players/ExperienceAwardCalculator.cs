using MochiMud.WebApp.Mobs;

namespace MochiMud.WebApp.Players
{
    public static class ExperienceAwardCalculator
    {
        public const double MinAwardMultiplier = 0.1;
        public const double MaxBonusMultiplier = 2.0;

        public static double MaxAwardMultiplier(int playerLevel)
        {
            return 1.0
                + (Player.MaximumLevel - playerLevel)
                / (double)(Player.MaximumLevel - 1)
                * (MaxBonusMultiplier - 1.0);
        }

        public static int CalculateMobKillExperience(Mob mob, Player player, int participantCount)
        {
            if (participantCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(participantCount),
                    participantCount,
                    "Participant count must be greater than zero.");
            }

            if (player.Level >= Player.MaximumLevel)
            {
                return 0;
            }

            var ratio = mob.Level / (double)player.Level;
            var clampedRatio = Math.Clamp(ratio, MinAwardMultiplier, MaxAwardMultiplier(player.Level));
            var individualAward = (int)Math.Floor(mob.ExperiencePoints * clampedRatio);

            return individualAward / participantCount;
        }
    }
}
