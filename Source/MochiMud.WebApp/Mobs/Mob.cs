using MochiMud.WebApp.Characters;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Mobs
{
    public class Mob : Character
    {
        public Mob(string name)
            : this(name, WorldConstants.DefaultStartRoomId)
        {
        }

        public Mob(string name, Guid currentRoomId)
            : base(name)
        {
            CurrentRoomId = currentRoomId;
        }

        public Guid CurrentRoomId { get; set; }

        public bool IsAggressive { get; set; }

        public int Level { get; set; } = 1;

        public int ExperiencePoints { get; set; } = 100;
    }
}
