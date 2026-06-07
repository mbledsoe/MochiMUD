using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player
    {
        public Player(string name)
        {
            Name = name;
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
        }

        public string Name { get; }

        public Guid CurrentRoomId { get; set; }
    }
}
