using MochiMud.WebApp.Hubs;

namespace MochiMud.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMudCommandServices();
            builder.Services.AddMudGameLoopServices();
            builder.Services.AddMudMobServices();
            builder.Services.AddMudPlayerServices();
            builder.Services.AddMudRealtimeServices();
            builder.Services.AddMudWorldServices();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapHub<MudHub>("/mudHub");

            app.Run();
        }
    }
}
