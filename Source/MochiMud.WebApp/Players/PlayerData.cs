namespace MochiMud.WebApp.Players
{
    public sealed record PlayerData(
        Guid Id,
        string Name,
        int MaximumHitPoints,
        PlayerClass Class,
        int Level,
        int ExperiencePoints);
}
