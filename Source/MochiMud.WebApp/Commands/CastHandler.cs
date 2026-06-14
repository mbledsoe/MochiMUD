using MochiMud.WebApp.Players;
using MochiMud.WebApp.Spells;

namespace MochiMud.WebApp.Commands
{
    public class CastHandler : CommandHandlerBase
    {
        private readonly SpellRegistry spellRegistry;

        public CastHandler(SpellRegistry spellRegistry)
        {
            this.spellRegistry = spellRegistry;
        }

        public override bool CanExecuteInFight => true;

        public override string CommandName => "cast";

        public override async Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            var spellText = CommandTextParser.GetArguments(command);

            if (spellText is null)
            {
                await client.SendMessageAsync("Cast what?", cancellationToken);
                return;
            }

            if (!spellRegistry.TryMatchSpell(spellText, out var spell, out var targetName))
            {
                await client.SendMessageAsync("You don't know that spell.", cancellationToken);
                return;
            }

            if (player.Class != spell.RequiredClass)
            {
                await client.SendMessageAsync($"Only {spell.RequiredClass.ToString().ToLowerInvariant()}s can cast {spell.Name}.", cancellationToken);
                return;
            }

            if (!player.Mana.HasAtLeast(spell.ManaCost))
            {
                await client.SendMessageAsync("You do not have enough mana.", cancellationToken);
                return;
            }

            await spell.CastAsync(new SpellCastContext(player, targetName, client, cancellationToken));
        }
    }
}
