namespace MochiMud.Core.Workflows
{
	public interface IWorkflowManager
	{
		void CancelWorkflow(ActivePlayer activePlayer);
		void HandleInput(ActivePlayer player, string input);
		void StartWorkflow<TWorkflow>(ActivePlayer activePlayer) where TWorkflow : IWorkflow;
	}
}