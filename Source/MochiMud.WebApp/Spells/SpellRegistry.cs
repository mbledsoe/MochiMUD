namespace MochiMud.WebApp.Spells
{
    public class SpellRegistry
    {
        private readonly IReadOnlyCollection<ISpell> spells;

        public SpellRegistry(IEnumerable<ISpell> spells)
        {
            this.spells = spells.ToArray();
        }

        public bool TryMatchSpell(string input, out ISpell spell, out string? targetName)
        {
            var trimmedInput = input.Trim();

            foreach (var candidate in spells.OrderByDescending(spell => spell.Name.Length))
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
