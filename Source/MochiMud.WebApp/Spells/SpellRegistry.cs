using MochiMud.WebApp.Combat;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public class SpellRegistry
    {
        private static readonly SpellDefinition LightningBolt = new(
            "lightning bolt",
            PlayerClass.Mage,
            5,
            new DamageDiceSpecification(2, 6),
            SpellEffect.Damage);

        private static readonly SpellDefinition Cure = new(
            "cure",
            PlayerClass.Cleric,
            5,
            new DamageDiceSpecification(2, 6),
            SpellEffect.Heal);

        private static readonly IReadOnlyCollection<SpellDefinition> Spells = [LightningBolt, Cure];

        public bool TryMatchSpell(string input, out SpellDefinition spell, out string? targetName)
        {
            var trimmedInput = input.Trim();

            foreach (var candidate in Spells.OrderByDescending(spell => spell.Name.Length))
            {
                if (!trimmedInput.StartsWith(candidate.Name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (trimmedInput.Length > candidate.Name.Length
                    && !char.IsWhiteSpace(trimmedInput[candidate.Name.Length]))
                {
                    continue;
                }

                var remainingText = trimmedInput[candidate.Name.Length..].Trim();

                spell = candidate;
                targetName = string.IsNullOrWhiteSpace(remainingText) ? null : remainingText;
                return true;
            }

            spell = default!;
            targetName = null;
            return false;
        }
    }
}
