using System.Text;
using Microsoft.Extensions.Options;
using MochiMud.Core.Config;
using MochiMud.Core.World;

namespace MochiMud.Core.Workflows.Login
{
	public class LoginWorkflow : IWorkflow
	{
		private readonly IOptionsMonitor<GameOptions> gameOptions;
		private readonly IActivePlayerManager activePlayerManager;
		private readonly IPlayerLocationManager playerLocationManager;
		private readonly IRoomDataService roomDataService;
		private readonly IPlayerOutput playerOutput;
		private readonly IWorkflowManager workflowManager;

		// workflow state / context
		private LoginState state = LoginState.Pending;
		private Guid activePlayerId;

		public LoginWorkflow(
			IOptionsMonitor<GameOptions> gameOptions,
			IActivePlayerManager activePlayerManager,
			IPlayerLocationManager playerLocationManager,
			IRoomDataService roomDataService,
			IPlayerOutput playerOutput,
			IWorkflowManager workflowManager)
		{
			this.gameOptions = gameOptions;
			this.activePlayerManager = activePlayerManager;
			this.playerLocationManager = playerLocationManager;
			this.roomDataService = roomDataService;
			this.playerOutput = playerOutput;
			this.workflowManager = workflowManager;
		}

		public void Start(ActivePlayer activePlayer)
		{
			activePlayerId = activePlayer.Id;
			state = LoginState.WaitingForUsername;
			playerOutput.Send(activePlayer, "By what name shall you be known?");
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

			var sb = new StringBuilder();
			sb.AppendLine($"<div>{roomData.Title}</div>");
			sb.AppendLine($"<div>{roomData.Description}</div>");

			playerOutput.Send(activePlayer, "You are entering the realm!");
			playerOutput.Send(activePlayer, sb.ToString());

			workflowManager.CompleteWorkflow(activePlayer);
		}
	}
}
