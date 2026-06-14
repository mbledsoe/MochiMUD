using MochiMud.WebApp.Characters;

namespace MochiMud.WebApp.Players
{
    public sealed record PlayerData(
        Guid Id,
        string Name,
        ResourcePool HitPoints,
        PlayerClass Class,
        int Level,
        int ExperiencePoints,
        ResourcePool Mana,
        bool AutoExits);
}
