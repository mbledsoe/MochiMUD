using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Mobs
{
    public class Mob
    {
        public Mob(string name)
            : this(name, WorldConstants.DefaultStartRoomId)
        {
        }

        public Mob(string name, Guid currentRoomId)
        {
            Name = name;
            CurrentRoomId = currentRoomId;
            HitPoints = 50;
        }

        public string Name { get; }

        public Guid CurrentRoomId { get; set; }

        public int HitPoints { get; set; }
    }
}
