using System.Numerics;
using System.Text;
using Microsoft.Extensions.Options;
using MochiMud.Core.Config;
using MochiMud.Core.Output;
using MochiMud.Core.World;

namespace MochiMud.Core.Workflows.Login
{
	public class LoginWorkflow : IWorkflow
	{
		private readonly IOptionsMonitor<GameOptions> gameOptions;
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IRoomDataService roomDataService;
		private readonly IOutputQueue outputQueue;
		private readonly IWorkflowManager workflowManager;
		private readonly IOutputBuilder outputBuilder;

		// workflow state / context
		private LoginState state = LoginState.Pending;
		private Guid activePlayerId;

		public LoginWorkflow(
			IOptionsMonitor<GameOptions> gameOptions,
			IActivePlayerManager activePlayerManager,
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService,
			IOutputQueue outputQueue,
			IWorkflowManager workflowManager,
			IOutputBuilder outputBuilder)
		{
			this.gameOptions = gameOptions;
			this.activePlayerManager = activePlayerManager;
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
			this.outputQueue = outputQueue;
			this.workflowManager = workflowManager;
			this.outputBuilder = outputBuilder;
		}

		public void Start(ActivePlayer activePlayer)
		{
			activePlayerId = activePlayer.Id;
			state = LoginState.WaitingForUsername;
			outputQueue.Enqueue(new MochiMudOutput(activePlayerId, "By what name shall you be known?"));
		}

		public void HandleInput(string input)
		{
			switch (state)
			{
				case LoginState.WaitingForUsername:
					HandleInputWaitingForUsername(input);
					break;
				case LoginState.WaitingForPassword:
					// todo
					break;
				default:
					throw new MochiMudException($"Unknown LoginState {state}");
			}
		}

		private void HandleInputWaitingForUsername(string input)
		{
			var activePlayer = activePlayerManager.GetById(activePlayerId);

			if (activePlayer == null)
			{
				return;
			}

			var startingRoomId = gameOptions.CurrentValue.StartingRoomId;
			playerLocationManager.MovePlayerToRoom(activePlayer, startingRoomId);
			var roomData = roomDataService.GetRoom(startingRoomId);

			outputQueue.Enqueue(new MochiMudOutput(activePlayer.Id, "You are entering the realm!"));
			outputQueue.Enqueue(new MochiMudOutput(activePlayer.Id, outputBuilder.BuildRoomOutput(roomData)));

			workflowManager.CompleteWorkflow(activePlayer);
		}
	}
}
