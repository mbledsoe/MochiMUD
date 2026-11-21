using Microsoft.Extensions.DependencyInjection.Extensions;
using MochiMud.Core;

namespace MochiMud.WebApp.Services
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddMochiMudWebApp(this IServiceCollection services)
		{
			services.AddSingleton<IPlayerOutput, SignalRPlayerOutput>();

			return services;
		}
	}
}
