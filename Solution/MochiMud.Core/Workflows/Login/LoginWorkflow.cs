namespace MochiMud.Core.Workflows.Login
{
	public class LoginWorkflow : IWorkflow
	{
		private readonly IActivePlayerManager playerConnectionManager;
		private readonly IPlayerOutput playerOutput;

		// workflow state / context
		private LoginState state = LoginState.Pending;

		public LoginWorkflow(
			IActivePlayerManager activePlayerManager,
			IPlayerOutput playerOutput)
		{
			this.playerConnectionManager = activePlayerManager;
			this.playerOutput = playerOutput;
		}

		public void Start(ActivePlayer player)
		{
			state = LoginState.WaitingForUsername;
			playerOutput.Send(player, "By what name shall you be known?");
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
		}
	}
}
