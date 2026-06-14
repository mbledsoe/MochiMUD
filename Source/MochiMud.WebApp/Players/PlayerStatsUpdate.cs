using MochiMud.WebApp.Characters;

namespace MochiMud.WebApp.Players
{
    public sealed record PlayerStatsUpdate(
        Guid PlayerId,
        ResourcePool HitPoints,
        ResourcePool Mana);
}
