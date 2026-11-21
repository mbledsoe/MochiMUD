namespace MochiMud.Core.Input
{
	public interface IInputQueue
	{
		IReadOnlyList<MochiMudInput> DequeueAllPending();
		void Enqueue(MochiMudInput input);
	}
}