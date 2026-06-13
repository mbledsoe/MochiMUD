using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Combat;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player : Character
    {
        public Player(Guid id, string name)
            : base(name)
        {
            Id = id;
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
            Weapon = new Weapon("A rusty epee", new DamageDiceSpecification(1, 6));
        }

        public Player(string name)
            : this(Guid.NewGuid(), name)
        {
        }

        public Guid Id { get; }

        public Guid CurrentRoomId { get; set; }

        public static Player FromData(PlayerData data)
        {
            return new Player(data.Id, data.Name)
            {
                MaximumHitPoints = data.MaximumHitPoints,
                HitPoints = data.MaximumHitPoints,
            };
        }
    }
}
