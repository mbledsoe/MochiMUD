using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace MochiMud.Core.Workflows
{
	public class WorkflowManager : IWorkflowManager
	{
		// Only one active workflow per active player
		private readonly ConcurrentDictionary<Guid, IWorkflow> activeWorkflows = new();
		private readonly IServiceProvider serviceProvider;

		public WorkflowManager(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public void StartWorkflow<TWorkflow>(ActivePlayer activePlayer) where TWorkflow : IWorkflow
		{
			var workflow = ActivatorUtilities.CreateInstance<TWorkflow>(serviceProvider);
			activeWorkflows[activePlayer.Id] = workflow;

			workflow.Start(activePlayer);
		}

		public void HandleInput(ActivePlayer player, string input)
		{
			activeWorkflows.TryGetValue(player.Id, out var workflow);

			if (workflow == null)
			{
				throw new MochiMudException("No active workflow!");
			}

			workflow.HandleInput(input);
		}

		public void CancelWorkflow(ActivePlayer activePlayer)
		{
			activeWorkflows.TryRemove(activePlayer.Id, out var _);
		}

		public bool HasWorkflow(ActivePlayer activePlayer)
		{
			return activeWorkflows.ContainsKey(activePlayer.Id);
		}

		public void CompleteWorkflow(ActivePlayer activePlayer)
		{
			activeWorkflows.TryRemove(activePlayer.Id, out var _);
		}
	}
}
