using Microsoft.Extensions.DependencyInjection.Extensions;
using MochiMud.Core;
using MochiMud.Core.Output;

namespace MochiMud.WebApp.Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddMochiMudWebApp(this IServiceCollection services)
		{
			services.AddSingleton<IOutputSender, SignalROutputSender>();
			services.AddHostedService<GameLoopHostedService>();

			return services;
		}
	}
}
