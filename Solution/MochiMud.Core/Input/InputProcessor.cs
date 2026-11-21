using MochiMud.Core.Commands;
using MochiMud.Core.Workflows;

namespace MochiMud.Core.Input
{
	public class InputProcessor : IInputProcessor
	{
		private readonly IInputQueue inputQueue;
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IWorkflowManager workflowManager;
		private readonly ICommandProvider commandProvider;
		private readonly IPlayerOutput playerOutput;

		public InputProcessor(
			IInputQueue inputQueue,
			IActivePlayerManager activePlayerManager,
			IWorkflowManager workflowManager,
			ICommandProvider commandProvider,
			IPlayerOutput playerOutput)
		{
			this.inputQueue = inputQueue;
			this.activePlayerManager = activePlayerManager;
			this.workflowManager = workflowManager;
			this.commandProvider = commandProvider;
			this.playerOutput = playerOutput;
		}

		public void ProcessPendingInput()
		{
			// Input is serialized in the queue and processed one command at a time.
			foreach (var input in inputQueue.DequeueAllPending())
			{
				ProcessInput(input);
			}
		}

		private void ProcessInput(MochiMudInput input)
		{
			var activePlayer = activePlayerManager.GetById(input.ActivePlayerId);

			if (activePlayer == null)
			{
				// todo: add logging
				return;
			}

			if (workflowManager.HasWorkflow(activePlayer))
			{
				workflowManager.HandleInput(activePlayer, input.Message);
			}
			else
			{
				var command = commandProvider.GetCommandByName(input.Message);

				if (command == null)
				{
					playerOutput.Send(activePlayer, $"Sorry, I don't understand that command!");
					return;
				}

				command.Execute(activePlayer, input);
			}
		}
	}
}
