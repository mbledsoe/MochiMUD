using MochiMud.Core.Input;

namespace MochiMud.Core
{
	public class GameLoop
	{
		private readonly IInputProcessor inputProcessor;

		public GameLoop(IInputProcessor inputProcessor)
		{
			this.inputProcessor = inputProcessor;
		}

		public async Task Run(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				inputProcessor.ProcessPendingInput();

				// todo: come back to this later
				await Task.Delay(1, ct);
			}
		}
	}
}
