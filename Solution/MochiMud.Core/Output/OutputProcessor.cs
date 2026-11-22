namespace MochiMud.Core.Output
{
	public class OutputProcessor : IOutputProcessor
	{
		private readonly IOutputQueue outputQueue;
		private readonly IOutputSender outputSender;

		public OutputProcessor(
			IOutputQueue outputQueue,
			IOutputSender outputSender)
		{
			this.outputQueue = outputQueue;
			this.outputSender = outputSender;
		}

		public void ProcessPendingOutput()
		{
			var pendingOutput = outputQueue.DequeueAllPending();

			foreach (var output in pendingOutput)
			{
				outputSender.Send(output);
			}
		}
	}
}
