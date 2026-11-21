namespace MochiMud.Core
{
	[Serializable]
	internal class MochiMudException : Exception
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