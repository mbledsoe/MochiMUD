using Microsoft.Extensions.DependencyInjection;
using MochiMud.Core.Commands;
using MochiMud.Core.Config;
using MochiMud.Core.Input;
using MochiMud.Core.Workflows;
using MochiMud.Core.Workflows.Login;
using MochiMud.Core.World;

namespace MochiMud.Core
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddMochiMudCore(
			this IServiceCollection services)
		{
			services.AddOptionsWithValidateOnStart<GameOptions>()
				.BindConfiguration(GameOptions.SectionName)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			// Data services
			services.AddSingleton<IRoomDataService, RoomDataService>();

			// Core game services
			services.AddSingleton<IActivePlayerManager, ActivePlayerManager>();
			services.AddSingleton<IPlayerLocationManager, PlayerLocationManager>();
			services.AddSingleton<IInputQueue, InputQueue>();
			services.AddSingleton<IInputProcessor, InputProcessor>();

			// Command processing
			services.AddSingleton<ICommand,QuitCommand>();
			services.AddSingleton<ICommandProvider, CommandProvider>();

			services.AddSingleton<GameLoop>();

			// Workflows
			services.AddSingleton<IWorkflowManager, WorkflowManager>();
			services.AddTransient<LoginWorkflow>();

			return services;
		}
	}
}
