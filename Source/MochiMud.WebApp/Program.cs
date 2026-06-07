using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<CommandProcessor>();
            builder.Services.AddSingleton<ICommandHandler, LookHandler>();
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
