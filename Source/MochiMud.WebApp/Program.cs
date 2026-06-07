using MochiMud.WebApp.Hubs;

namespace MochiMud.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapHub<MudHub>("/mudHub");

            app.Run();
        }
    }
}
