using MochiMud.WebApp.Combat;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public class LightningBoltSpell : ISpell
    {
        private static readonly DiceSpecification DamageDice = new(2, 6);

        private readonly FightService fightService;
        private readonly IMobDataService mobDataService;

        public LightningBoltSpell(FightService fightService, IMobDataService mobDataService)
        {
            this.fightService = fightService;
            this.mobDataService = mobDataService;
        }

        public string Name => "lightning bolt";

        public PlayerClass RequiredClass => PlayerClass.Mage;

        public int ManaCost => 5;

        public async Task CastAsync(SpellCastContext context)
        {
            if (context.TargetName is null)
            {
                await context.Client.SendMessageAsync($"Cast {Name} at what?", context.CancellationToken);
                return;
            }

            var target = await FindMobInRoomAsync(
                context.Caster.CurrentRoomId,
                context.TargetName,
                context.CancellationToken);

            if (target is null)
            {
                await context.Client.SendMessageAsync("You don't see that here.", context.CancellationToken);
                return;
            }

            await fightService.CastDamageSpellAsync(
                context.Caster,
                target,
                Name,
                ManaCost,
                DamageDice,
                context.CancellationToken);
        }

        private async Task<Mob?> FindMobInRoomAsync(
            Guid roomId,
            string targetName,
            CancellationToken cancellationToken)
        {
            var mobs = await mobDataService.GetMobsInRoomAsync(roomId, cancellationToken);

            return mobs.FirstOrDefault(mob => string.Equals(mob.Name, targetName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
