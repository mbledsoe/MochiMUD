using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MochiMud.WebApp.Connections;
using MochiMud.WebApp.Game;

namespace MochiMud.WebApp.Players
{
    public partial class PlayerCreationService
    {
        private const string NamePrompt = "Welcome! What would you like to name your character?";
        private const string ClassPrompt = "Choose a class: Warrior, Cleric, Mage, or Thief.";

        private readonly ConcurrentDictionary<string, CreationSession> sessionsByConnectionId = new();
        private readonly GameStateService gameStateService;
        private readonly ILogger<PlayerCreationService> logger;
        private readonly IPlayerStore playerStore;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;

        public PlayerCreationService(
            GameStateService gameStateService,
            ILogger<PlayerCreationService> logger,
            IPlayerStore playerStore,
            PlayerConnectionRegistry playerConnectionRegistry)
        {
            this.gameStateService = gameStateService;
            this.logger = logger;
            this.playerStore = playerStore;
            this.playerConnectionRegistry = playerConnectionRegistry;
        }

        public async Task BeginAsync(
            IClientConnection connection,
            CancellationToken cancellationToken = default)
        {
            sessionsByConnectionId[connection.ConnectionId] = new CreationSession(connection.AccountId);

            logger.LogInformation("Started player creation for connection {ConnectionId}.", connection.ConnectionId);

            await connection.Client.SendMessageAsync(NamePrompt, cancellationToken);
        }

        public bool IsInCreation(string connectionId)
        {
            return sessionsByConnectionId.ContainsKey(connectionId);
        }

        public void Abandon(string connectionId)
        {
            sessionsByConnectionId.TryRemove(connectionId, out _);
        }

        public async Task HandleInputAsync(
            IClientConnection connection,
            string input,
            CancellationToken cancellationToken = default)
        {
            if (!sessionsByConnectionId.TryGetValue(connection.ConnectionId, out var session))
            {
                return;
            }

            if (session.Step == CreationStep.ChooseClass)
            {
                await HandleClassInputAsync(connection, session, input, cancellationToken);
                return;
            }

            await HandleNameInputAsync(connection, session, input, cancellationToken);
        }

        private async Task HandleNameInputAsync(
            IClientConnection connection,
            CreationSession session,
            string input,
            CancellationToken cancellationToken)
        {
            var client = connection.Client;
            var name = (input ?? string.Empty).Trim();

            if (!NameRegex().IsMatch(name))
            {
                await client.SendMessageAsync(
                    "Names must be 3-20 characters using letters, digits, or underscore. Please choose another name.",
                    cancellationToken);
                return;
            }

            var reserved = await playerStore.TryReserveNameAsync(name, session.AccountId, cancellationToken);

            if (!reserved)
            {
                await client.SendMessageAsync(
                    "That name is already taken. Please choose another name.",
                    cancellationToken);
                return;
            }

            sessionsByConnectionId[connection.ConnectionId] = session with
            {
                Name = name,
                Step = CreationStep.ChooseClass,
            };

            await client.SendMessageAsync(ClassPrompt, cancellationToken);
        }

        private async Task HandleClassInputAsync(
            IClientConnection connection,
            CreationSession session,
            string input,
            CancellationToken cancellationToken)
        {
            var client = connection.Client;
            var className = (input ?? string.Empty).Trim();

            if (!TryParsePlayerClass(className, out var playerClass))
            {
                await client.SendMessageAsync(
                    "Classes must be Warrior, Cleric, Mage, or Thief. Please choose a class.",
                    cancellationToken);
                return;
            }

            var player = new Player(session.AccountId, session.Name!, playerClass);

            await playerStore.SaveAsync(player, cancellationToken);

            sessionsByConnectionId.TryRemove(connection.ConnectionId, out _);
            playerConnectionRegistry.AddOrUpdate(connection.ConnectionId, player);

            logger.LogInformation(
                "Created player {PlayerName} for account {AccountId}.",
                player.Name,
                session.AccountId);

            await client.SendMessageAsync($"Welcome to MochiMUD, {player.Name} the {player.Class}!", cancellationToken);

            await gameStateService.AddPlayerToGameAsync(player, client, cancellationToken);
        }

        private static bool TryParsePlayerClass(string className, out PlayerClass playerClass)
        {
            if (Enum.GetNames<PlayerClass>().Any(name => string.Equals(name, className, StringComparison.OrdinalIgnoreCase)))
            {
                return Enum.TryParse(className, ignoreCase: true, out playerClass);
            }

            playerClass = default;
            return false;
        }

        [GeneratedRegex("^[A-Za-z0-9_]{3,20}$")]
        private static partial Regex NameRegex();

        private enum CreationStep
        {
            ChooseName,
            ChooseClass
        }

        private sealed record CreationSession(
            Guid AccountId,
            CreationStep Step = CreationStep.ChooseName,
            string? Name = null);
    }
}
