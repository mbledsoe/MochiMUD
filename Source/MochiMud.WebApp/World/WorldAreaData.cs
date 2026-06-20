using MochiMud.WebApp.Combat;

namespace MochiMud.WebApp.World
{
    public sealed record WorldAreaData(
        Room[] Rooms,
        MobDefinitionData[] MobDefinitions,
        MobSpawnData[] MobSpawns);

    public sealed record MobDefinitionData(
        string Id,
        string Name,
        DiceSpecification BareHandDamage,
        bool IsAggressive,
        int Level = 1,
        int ExperiencePoints = 100);

    public sealed record MobSpawnData(
        string MobDefinitionId,
        Guid RoomId);
}
