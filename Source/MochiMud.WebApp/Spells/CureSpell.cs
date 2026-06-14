using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Combat;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public class CureSpell : ISpell
    {
        private static readonly DiceSpecification HealingDice = new(2, 6);

        private readonly FightService fightService;
        private readonly IMobDataService mobDataService;
        private readonly IPlayerDataService playerDataService;

        public CureSpell(
            FightService fightService,
            IMobDataService mobDataService,
            IPlayerDataService playerDataService)
        {
            this.fightService = fightService;
            this.mobDataService = mobDataService;
            this.playerDataService = playerDataService;
        }

        public string Name => "cure";

        public PlayerClass RequiredClass => PlayerClass.Cleric;

        public int ManaCost => 5;

        public async Task CastAsync(SpellCastContext context)
        {
            if (context.TargetName is null)
            {
                await context.Client.SendMessageAsync($"Cast {Name} at what?", context.CancellationToken);
                return;
            }

            var target = await FindCharacterInRoomAsync(
                context.Caster,
                context.TargetName,
                context.CancellationToken);

            if (target is null)
            {
                await context.Client.SendMessageAsync("You don't see that here.", context.CancellationToken);
                return;
            }

            await fightService.CastHealingSpellAsync(
                context.Caster,
                target,
                Name,
                ManaCost,
                HealingDice,
                context.CancellationToken);
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

            var mobs = await mobDataService.GetMobsInRoomAsync(caster.CurrentRoomId, cancellationToken);

            return mobs.FirstOrDefault(mob => string.Equals(mob.Name, targetName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
