using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public interface ISpell
    {
        string Name { get; }

        PlayerClass RequiredClass { get; }

        int ManaCost { get; }

        Task CastAsync(SpellCastContext context);
    }

    public sealed record SpellCastContext(
        Player Caster,
        string? TargetName,
        ICommandClient Client,
        CancellationToken CancellationToken);
}
