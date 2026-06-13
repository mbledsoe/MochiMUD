using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MochiMud.WebApp.Connections;
using MochiMud.WebApp.Game;

namespace MochiMud.WebApp.Players
{
    public partial class PlayerCreationService
    {
        private const string NamePrompt = "Welcome! What would you like to name your character?";

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

            var player = new Player(session.AccountId, name);

            await playerStore.SaveAsync(player, cancellationToken);

            sessionsByConnectionId.TryRemove(connection.ConnectionId, out _);
            playerConnectionRegistry.AddOrUpdate(connection.ConnectionId, player);

            logger.LogInformation(
                "Created player {PlayerName} for account {AccountId}.",
                player.Name,
                session.AccountId);

            await client.SendMessageAsync($"Welcome to MochiMUD, {player.Name}!", cancellationToken);

            await gameStateService.AddPlayerToGameAsync(player, client, cancellationToken);
        }

        [GeneratedRegex("^[A-Za-z0-9_]{3,20}$")]
        private static partial Regex NameRegex();

        private sealed record CreationSession(Guid AccountId);
    }
}
