namespace MochiMud.Core
{
	public class ActivePlayer
	{
		public ActivePlayer(string connectionId)
		{
			Id = Guid.NewGuid();
			ConnectionId = connectionId;
		}

		public Guid Id { get; }
		public string ConnectionId { get; }
	}
}
