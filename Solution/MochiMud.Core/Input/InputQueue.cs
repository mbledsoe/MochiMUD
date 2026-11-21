using System.Collections.Concurrent;

namespace MochiMud.Core.Input
{
	public class InputQueue : IInputQueue
	{
		private ConcurrentQueue<MochiMudInput> queue = new();

		public void Enqueue(MochiMudInput input)
		{
			queue.Enqueue(input);
		}

		public IReadOnlyList<MochiMudInput> DequeueAllPending()
		{
			var pendingQueue = Interlocked.Exchange(ref queue, new ConcurrentQueue<MochiMudInput>());
			var pendingList = new List<MochiMudInput>();

			while (pendingQueue.TryDequeue(out var pendingInput))
			{
				pendingList.Add(pendingInput);
			}

			return pendingList.AsReadOnly();
		}
	}
}
