using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.GameLoop;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMudCommandServices(this IServiceCollection services)
        {
            services.AddSingleton<CommandExecutionQueue>();
            services.AddSingleton<CommandExecutor>();
            services.AddSingleton<CommandProcessor>();
            services.AddSingleton<FightService>();
            services.AddSingleton<MoveService>();
            services.AddSingleton<RoomPresenter>();
            services.AddSingleton<ICommandHandler, FleeHandler>();
            services.AddSingleton<ICommandHandler, KillHandler>();
            services.AddSingleton<ICommandHandler, LookHandler>();
            services.AddSingleton<ICommandHandler, MoveHandler>();

            services.AddDirectionCommand("north", Direction.North);
            services.AddDirectionCommand("south", Direction.South);
            services.AddDirectionCommand("east", Direction.East);
            services.AddDirectionCommand("west", Direction.West);

            return services;
        }

        public static IServiceCollection AddMudGameLoopServices(this IServiceCollection services)
        {
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
            services.AddSingleton<PlayerConnectionRegistry>();

            return services;
        }

        public static IServiceCollection AddMudRealtimeServices(this IServiceCollection services)
        {
            services.AddSingleton<ICommandNotificationService, SignalRCommandNotificationService>();
            services.AddSignalR();

            return services;
        }

        public static IServiceCollection AddMudWorldServices(this IServiceCollection services)
        {
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
