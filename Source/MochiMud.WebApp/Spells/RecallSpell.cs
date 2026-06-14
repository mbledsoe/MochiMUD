using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Spells
{
    public class RecallSpell : ISpell
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly IPlayerDataService playerDataService;
        private readonly RecallService recallService;

        public RecallSpell(
            ICommandNotificationService commandNotificationService,
            IPlayerDataService playerDataService,
            RecallService recallService)
        {
            this.commandNotificationService = commandNotificationService;
            this.playerDataService = playerDataService;
            this.recallService = recallService;
        }

        public string Name => "recall";

        public PlayerClass RequiredClass => PlayerClass.Mage;

        public int ManaCost => 5;

        public async Task CastAsync(SpellCastContext context)
        {
            if (context.Caster.State == CharacterState.Fighting)
            {
                await context.Client.SendMessageAsync("You cannot do that while you are in a fight.", context.CancellationToken);
                return;
            }

            var target = FindTarget(context.Caster, context.TargetName);

            if (target is null)
            {
                await context.Client.SendMessageAsync("You cannot recall that player.", context.CancellationToken);
                return;
            }

            if (target.State == CharacterState.Fighting)
            {
                await context.Client.SendMessageAsync($"{target.Name} is fighting and cannot be recalled.", context.CancellationToken);
                return;
            }

            if (target.State == CharacterState.Dead)
            {
                await context.Client.SendMessageAsync($"{target.Name} cannot be recalled while dead.", context.CancellationToken);
                return;
            }

            context.Caster.Mana = context.Caster.Mana.Spend(ManaCost);
            await commandNotificationService.SendPlayerStatsUpdateAsync(context.Caster, context.CancellationToken);

            if (ReferenceEquals(target, context.Caster))
            {
                await recallService.RecallAsync(context.Caster, context.Client, context.CancellationToken);
                return;
            }

            await context.Client.SendMessageAsync($"You cast {Name} on {target.Name}.", context.CancellationToken);
            await recallService.RecallAsync(target, null, context.CancellationToken);
        }

        private Player? FindTarget(Player caster, string? targetName)
        {
            if (targetName is null
                || string.Equals(targetName, "self", StringComparison.OrdinalIgnoreCase)
                || string.Equals(targetName, "me", StringComparison.OrdinalIgnoreCase))
            {
                return caster;
            }

            return playerDataService
                .GetPlayers()
                .FirstOrDefault(player => string.Equals(player.Name, targetName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
