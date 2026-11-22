using MochiMud.Core;

namespace MochiMud.WebApp.Services
{
	public class GameLoopHostedService : BackgroundService
	{
		private readonly GameLoop gameLoop;

		public GameLoopHostedService(GameLoop gameLoop)
		{
			this.gameLoop = gameLoop;
		}

		protected override async Task ExecuteAsync(CancellationToken ct)
		{
			try
			{
				await gameLoop.Run(ct);
			}
			catch (Exception)
			{

				// todo: log unhandled exception from the game loop and shut down.
			}			
		}
	}
}
