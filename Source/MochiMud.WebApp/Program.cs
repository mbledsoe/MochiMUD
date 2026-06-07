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

            builder.Services.AddSingleton<CommandProcessor>();
            builder.Services.AddSingleton<RoomPresenter>();
            builder.Services.AddSingleton<ICommandHandler, LookHandler>();
            builder.Services.AddSingleton<ICommandHandler, MoveHandler>();
            builder.Services.AddSingleton<IMobDataService, StaticMobDataService>();
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
