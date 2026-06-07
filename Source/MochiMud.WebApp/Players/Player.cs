using MochiMud.WebApp.Combat;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player
    {
        private int isInFight;

        public Player(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
            HitPoints = 50;
            Weapon = new Weapon("A rusty epee", new DamageDiceSpecification(1, 6));
        }

        public Guid Id { get; }

        public string Name { get; }

        public Guid CurrentRoomId { get; set; }

        public int HitPoints { get; set; }

        public Weapon Weapon { get; }

        public bool IsInFight => Volatile.Read(ref isInFight) == 1;

        public void EndFight()
        {
            Volatile.Write(ref isInFight, 0);
        }

        public bool TryStartFight()
        {
            return Interlocked.CompareExchange(ref isInFight, 1, 0) == 0;
        }
    }
}
