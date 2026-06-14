using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.Spells;

namespace MochiMud.WebApp.Commands
{
    public class CastHandler : CommandHandlerBase
    {
        private readonly FightService fightService;
        private readonly IMobDataService mobDataService;
        private readonly IPlayerDataService playerDataService;
        private readonly SpellRegistry spellRegistry;

        public CastHandler(
            FightService fightService,
            IMobDataService mobDataService,
            IPlayerDataService playerDataService,
            SpellRegistry spellRegistry)
        {
            this.fightService = fightService;
            this.mobDataService = mobDataService;
            this.playerDataService = playerDataService;
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

            if (targetName is null)
            {
                await client.SendMessageAsync($"Cast {spell.Name} at what?", cancellationToken);
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

            if (spell.Effect == SpellEffect.Damage)
            {
                var target = await FindMobInRoomAsync(player.CurrentRoomId, targetName, cancellationToken);

                if (target is null)
                {
                    await client.SendMessageAsync("You don't see that here.", cancellationToken);
                    return;
                }

                await fightService.CastDamageSpellAsync(player, target, spell, cancellationToken);
                return;
            }

            var healingTarget = await FindCharacterInRoomAsync(player, targetName, cancellationToken);

            if (healingTarget is null)
            {
                await client.SendMessageAsync("You don't see that here.", cancellationToken);
                return;
            }

            await fightService.CastHealingSpellAsync(player, healingTarget, spell, cancellationToken);
        }

        private async Task<Mob?> FindMobInRoomAsync(
            Guid roomId,
            string targetName,
            CancellationToken cancellationToken)
        {
            var mobs = await mobDataService.GetMobsInRoomAsync(roomId, cancellationToken);

            return mobs.FirstOrDefault(mob => string.Equals(mob.Name, targetName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<Character?> FindCharacterInRoomAsync(
            Player caster,
            string targetName,
            CancellationToken cancellationToken)
        {
            if (string.Equals(targetName, "self", StringComparison.OrdinalIgnoreCase)
                || string.Equals(targetName, "me", StringComparison.OrdinalIgnoreCase))
            {
                return caster;
            }

            var player = playerDataService
                .GetPlayersInRoom(caster.CurrentRoomId)
                .FirstOrDefault(player => string.Equals(player.Name, targetName, StringComparison.OrdinalIgnoreCase));

            if (player is not null)
            {
                return player;
            }

            return await FindMobInRoomAsync(caster.CurrentRoomId, targetName, cancellationToken);
        }
    }
}
