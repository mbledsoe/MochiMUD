using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Combat;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player : Character
    {
        public Player(string name)
            : base(name)
        {
            Id = Guid.NewGuid();
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
            Weapon = new Weapon("A rusty epee", new DamageDiceSpecification(1, 6));
        }

        public Guid Id { get; }

        public Guid CurrentRoomId { get; set; }
    }
}
