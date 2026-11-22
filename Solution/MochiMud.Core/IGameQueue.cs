namespace MochiMud.Core
{
	public interface IGameQueue<TQueueItem>
	{
		IReadOnlyList<TQueueItem> DequeueAllPending();
		void Enqueue(TQueueItem queueItem);
	}
}