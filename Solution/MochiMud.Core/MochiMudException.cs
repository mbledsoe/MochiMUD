namespace MochiMud.Core
{
	[Serializable]
	public class MochiMudException : Exception
	{
		public MochiMudException()
		{
		}

		public MochiMudException(string? message) : base(message)
		{
		}

		public MochiMudException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}