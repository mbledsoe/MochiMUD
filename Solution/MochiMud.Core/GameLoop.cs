using MochiMud.Core.Input;
using MochiMud.Core.Output;

namespace MochiMud.Core
{
	public class GameLoop
	{
		private readonly IInputProcessor inputProcessor;
		private readonly IOutputProcessor outputProcessor;

		public GameLoop(
			IInputProcessor inputProcessor,
			IOutputProcessor outputProcessor)
		{
			this.inputProcessor = inputProcessor;
			this.outputProcessor = outputProcessor;
		}

		public async Task Run(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				inputProcessor.ProcessPendingInput();

				// todo: update game state here

				outputProcessor.ProcessPendingOutput();

				// todo: come back to this later
				await Task.Delay(1, ct);
			}
		}
	}
}
