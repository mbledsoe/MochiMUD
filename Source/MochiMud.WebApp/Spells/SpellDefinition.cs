using MochiMud.WebApp.Combat;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public sealed record SpellDefinition(
        string Name,
        PlayerClass RequiredClass,
        int ManaCost,
        DamageDiceSpecification EffectDice,
        SpellEffect Effect);
}
