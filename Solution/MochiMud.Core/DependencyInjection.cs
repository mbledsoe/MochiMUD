using Microsoft.Extensions.DependencyInjection;
using MochiMud.Core.Commands;
using MochiMud.Core.Config;
using MochiMud.Core.Input;
using MochiMud.Core.Output;
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
			services.AddSingleton<IRoomDataStore, StaticRoomDataStore>();
			services.AddSingleton<IRoomDataService>(provider =>
			{
				var roomDataStore = provider.GetRequiredService<IRoomDataStore>();
				var roomDataService = new RoomDataService();
				roomDataService.Initialize(roomDataStore);

				return roomDataService;
			});

			// Core game services
			services.AddSingleton<IActivePlayerManager, ActivePlayerManager>();
			services.AddSingleton<IPlayerLocationManager, PlayerLocationManager>();

			services.AddSingleton<IInputQueue, InputQueue>();
			services.AddSingleton<IInputProcessor, InputProcessor>();

			services.AddSingleton<IOutputQueue, OutputQueue>();
			services.AddSingleton<IOutputProcessor, OutputProcessor>();
			services.AddSingleton<IOutputBuilder, HtmlOutputBuilder>();

			// Command processing
			services.AddSingleton<ICommand, QuitCommand>();
			services.AddSingleton<ICommand, LookCommand>();
			services.AddSingleton(sp => (ICommand)ActivatorUtilities.CreateInstance(sp, typeof(MoveCommand), Direction.North));
			services.AddSingleton(sp => (ICommand)ActivatorUtilities.CreateInstance(sp, typeof(MoveCommand), Direction.South));
			services.AddSingleton(sp => (ICommand)ActivatorUtilities.CreateInstance(sp, typeof(MoveCommand), Direction.East));
			services.AddSingleton(sp => (ICommand)ActivatorUtilities.CreateInstance(sp, typeof(MoveCommand), Direction.West));
			services.AddSingleton<ICommand, ExitsCommand>();
			services.AddSingleton<ICommand, HelpCommand>();

			services.AddSingleton<ICommandCatalog, CommandCatalog>();
			services.AddSingleton<ICommandProvider, CommandProvider>();

			services.AddSingleton<GameLoop>();

			// Workflows
			services.AddSingleton<IWorkflowManager, WorkflowManager>();
			services.AddTransient<LoginWorkflow>();

			return services;
		}
	}
}
