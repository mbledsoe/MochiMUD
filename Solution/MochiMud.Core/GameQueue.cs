using System.Collections.Concurrent;

namespace MochiMud.Core
{
	public abstract class GameQueue<TQueueItem> : IGameQueue<TQueueItem>
	{
		private ConcurrentQueue<TQueueItem> queue = new();

		public IReadOnlyList<TQueueItem> DequeueAllPending()
		{
			var pendingQueue = Interlocked.Exchange(ref queue, new ConcurrentQueue<TQueueItem>());
			var pendingList = new List<TQueueItem>();

			while (pendingQueue.TryDequeue(out var pendingItem))
			{
				pendingList.Add(pendingItem);
			}

			return pendingList.AsReadOnly();
		}

		public void Enqueue(TQueueItem queueItem)
		{
			queue.Enqueue(queueItem);
		}
	}
}