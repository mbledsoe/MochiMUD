namespace MochiMud.Core.Workflows
{
	public interface IWorkflow
	{
		void HandleInput(string input);
		
		void Start(ActivePlayer player);
	}
}
