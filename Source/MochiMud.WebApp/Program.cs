using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<CommandExecutionQueue>();
            builder.Services.AddSingleton<ICommandNotificationService, SignalRCommandNotificationService>();
            builder.Services.AddSingleton<CommandProcessor>();
            builder.Services.AddSingleton<MoveService>();
            builder.Services.AddSingleton<RoomPresenter>();
            builder.Services.AddSingleton<ICommandHandler, LookHandler>();
            builder.Services.AddSingleton<ICommandHandler, MoveHandler>();

            builder.Services.AddSingleton<ICommandHandler>(serviceProvider => new DirectionMoveHandler(
                "north",
                Direction.North,
                serviceProvider.GetRequiredService<ILogger<DirectionMoveHandler>>(),
                serviceProvider.GetRequiredService<MoveService>()));

            builder.Services.AddSingleton<ICommandHandler>(serviceProvider => new DirectionMoveHandler(
                "south",
                Direction.South,
                serviceProvider.GetRequiredService<ILogger<DirectionMoveHandler>>(),
                serviceProvider.GetRequiredService<MoveService>()));

            builder.Services.AddSingleton<ICommandHandler>(serviceProvider => new DirectionMoveHandler(
                "east",
                Direction.East,
                serviceProvider.GetRequiredService<ILogger<DirectionMoveHandler>>(),
                serviceProvider.GetRequiredService<MoveService>()));

            builder.Services.AddSingleton<ICommandHandler>(serviceProvider => new DirectionMoveHandler(
                "west",
                Direction.West,
                serviceProvider.GetRequiredService<ILogger<DirectionMoveHandler>>(),
                serviceProvider.GetRequiredService<MoveService>()));
            builder.Services.AddSingleton<IMobDataService, StaticMobDataService>();
            builder.Services.AddSingleton<IPlayerDataService, PlayerDataService>();
            builder.Services.AddSingleton<PlayerConnectionRegistry>();
            builder.Services.AddSingleton<IWorldDataService, StaticWorldDataService>();
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapHub<MudHub>("/mudHub");

            app.Run();
        }
    }
}
