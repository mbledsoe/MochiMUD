namespace MochiMud.WebApp.Players
{
    public sealed record PlayerStatsUpdate(Guid PlayerId, int CurrentHitPoints, int MaximumHitPoints);
}
