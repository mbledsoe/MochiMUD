namespace MochiMud.WebApp.Players
{
    public static class ExperienceTable
    {
        public static int XpToNextLevel(int level)
        {
            if (level < Player.MinimumLevel || level >= Player.MaximumLevel)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(level),
                    level,
                    $"Experience requirement is only defined for levels {Player.MinimumLevel} through {Player.MaximumLevel - 1}.");
            }

            return 100 * level * level;
        }
    }
}
