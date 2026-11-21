using System.ComponentModel.DataAnnotations;

namespace MochiMud.Core.Config
{
	public class GameOptions
	{
		public static string SectionName => "Game";

		[Required]
		public Guid StartingRoomId { get; } = Guid.Parse("B3D665FB-CD35-4AE5-AACF-C873CA975485");
	}
}
