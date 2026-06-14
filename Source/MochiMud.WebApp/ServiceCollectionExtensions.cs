using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MochiMud.WebApp.Authentication;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Connections;
using MochiMud.WebApp.Game;
using MochiMud.WebApp.GameLoop;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.Spells;
using MochiMud.WebApp.Storage;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMudAuthServices(this IServiceCollection services)
        {
            services.AddSingleton<IAccountStore, JsonAccountStore>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<AccountService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddMudCommandServices(this IServiceCollection services)
        {
            services.AddSingleton<CommandExecutionQueue>();
            services.AddSingleton<CommandExecutor>();
            services.AddSingleton<CommandProcessor>();
            services.AddSingleton<FightService>();
            services.AddSingleton<MoveService>();
            services.AddSingleton<RecallService>();
            services.AddSingleton<ExitsPresenter>();
            services.AddSingleton<RoomPresenter>();
            services.AddSingleton<SpellRegistry>();
            services.AddSingleton<ISpell, CureSpell>();
            services.AddSingleton<ISpell, LightningBoltSpell>();
            services.AddSingleton<ISpell, RecallSpell>();
            services.AddSingleton<ICommandHandler, AutoExitsHandler>();
            services.AddSingleton<ICommandHandler, CastHandler>();
            services.AddSingleton<ICommandHandler, ExitsHandler>();
            services.AddSingleton<ICommandHandler, FleeHandler>();
            services.AddSingleton<ICommandHandler, FollowHandler>();
            services.AddSingleton<ICommandHandler, HelpHandler>();
            services.AddSingleton<ICommandHandler, KillHandler>();
            services.AddSingleton<ICommandHandler, LookHandler>();
            services.AddSingleton<ICommandHandler, MoveHandler>();
            services.AddSingleton<ICommandHandler, RecallHandler>();
            services.AddSingleton<ICommandHandler, ReviveHandler>();

            services.AddDirectionCommand("n", Direction.North);
            services.AddDirectionCommand("north", Direction.North);
            services.AddDirectionCommand("s", Direction.South);
            services.AddDirectionCommand("south", Direction.South);
            services.AddDirectionCommand("e", Direction.East);
            services.AddDirectionCommand("east", Direction.East);
            services.AddDirectionCommand("w", Direction.West);
            services.AddDirectionCommand("west", Direction.West);

            return services;
        }

        public static IServiceCollection AddMudConnectionServices(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionManager>();

            return services;
        }

        public static IServiceCollection AddMudGameServices(this IServiceCollection services)
        {
            services.AddSingleton<GameStateService>();
            services.AddSingleton<WelcomeBannerProvider>();
            services.AddHostedService<GameLoopService>();

            return services;
        }

        public static IServiceCollection AddMudMobServices(this IServiceCollection services)
        {
            services.AddSingleton<IMobDataService, StaticMobDataService>();

            return services;
        }

        public static IServiceCollection AddMudPlayerServices(this IServiceCollection services)
        {
            services.AddSingleton<IPlayerDataService, PlayerDataService>();
            services.AddSingleton<IPlayerStore, JsonPlayerStore>();
            services.AddSingleton<PlayerConnectionRegistry>();
            services.AddSingleton<PlayerCreationService>();
            services.AddSingleton<PlayerGroupService>();

            return services;
        }

        public static IServiceCollection AddMudRealtimeServices(this IServiceCollection services)
        {
            services.AddSingleton<ICommandNotificationService, SignalRCommandNotificationService>();
            services.AddSignalR();

            return services;
        }

        public static IServiceCollection AddMudStorageServices(this IServiceCollection services)
        {
            services.AddOptions<DataStorageOptions>()
                .BindConfiguration(DataStorageOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<DataDirectoryProvider>();

            return services;
        }

        public static IServiceCollection AddMudWorldServices(this IServiceCollection services)
        {
            services.AddOptions<WorldFileStorageOptions>()
                .BindConfiguration(WorldFileStorageOptions.SectionName)
                .Validate(
                    options => Enum.IsDefined(options.Provider)
                        && (options.Provider != WorldFileStorageProvider.AzureBlobStorage
                            || (!string.IsNullOrWhiteSpace(options.AzureBlobStorage.ContainerName)
                                && (!string.IsNullOrWhiteSpace(options.AzureBlobStorage.ConnectionString)
                                    || options.AzureBlobStorage.ServiceUri is not null))),
                    "World file storage configuration is invalid.")
                .ValidateOnStart();

            services.AddSingleton<IWorldFileStore>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<WorldFileStorageOptions>>().Value;

                return options.Provider switch
                {
                    WorldFileStorageProvider.LocalFileSystem =>
                        ActivatorUtilities.CreateInstance<LocalWorldFileStore>(serviceProvider),
                    WorldFileStorageProvider.AzureBlobStorage =>
                        ActivatorUtilities.CreateInstance<AzureBlobWorldFileStore>(serviceProvider),
                    _ => throw new InvalidOperationException($"Unsupported world file storage provider: {options.Provider}"),
                };
            });

            services.AddSingleton<JsonWorldAreaManifestLoader>();
            services.AddSingleton<JsonWorldAreaLoader>();
            services.AddSingleton<IWorldDataService, StaticWorldDataService>();

            return services;
        }

        private static IServiceCollection AddDirectionCommand(
            this IServiceCollection services,
            string commandName,
            Direction direction)
        {
            services.AddSingleton<ICommandHandler>(serviceProvider => new DirectionMoveHandler(
                commandName,
                direction,
                serviceProvider.GetRequiredService<ILogger<DirectionMoveHandler>>(),
                serviceProvider.GetRequiredService<MoveService>()));

            return services;
        }
    }
}
