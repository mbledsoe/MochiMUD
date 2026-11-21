using Microsoft.Extensions.DependencyInjection;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.Login;

namespace MochiMud.Core
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddMochiMudCore(
			this IServiceCollection services)
		{
			// Core services
			services.AddSingleton<IActivePlayerManager, ActivePlayerManager>();

			// Workflows
			services.AddSingleton<IWorkflowManager, WorkflowManager>();
			services.AddTransient<LoginWorkflow>();

			return services;
		}
	}
}
