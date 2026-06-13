using MochiMud.WebApp.Hubs;

namespace MochiMud.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddMudAuthServices();
            builder.Services.AddMudCommandServices();
            builder.Services.AddMudConnectionServices();
            builder.Services.AddMudGameServices();
            builder.Services.AddMudMobServices();
            builder.Services.AddMudPlayerServices();
            builder.Services.AddMudRealtimeServices();
            builder.Services.AddMudStorageServices();
            builder.Services.AddMudWorldServices();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<MudHub>("/mudHub");

            app.Run();
        }
    }
}
