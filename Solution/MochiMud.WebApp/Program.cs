using MochiMud.Core;
using MochiMud.WebApp.Hubs;
using MochiMud.WebApp.Services;

namespace MochiMud.WebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddMochiMudCore();
			builder.Services.AddMochiMudWebApp();

			builder.Services.AddSignalR();
			
			var app = builder.Build();

			app.MapHub<MochiMudHub>($"/{nameof(MochiMudHub)}");

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.Run();
		}
	}
}
