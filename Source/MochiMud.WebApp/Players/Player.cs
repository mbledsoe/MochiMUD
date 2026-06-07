using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player
    {
        public Player(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
        }

        public Guid Id { get; }

        public string Name { get; }

        public Guid CurrentRoomId { get; set; }
    }
}
