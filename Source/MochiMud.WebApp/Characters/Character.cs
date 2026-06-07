using MochiMud.WebApp.Combat;

namespace MochiMud.WebApp.Characters
{
    public abstract class Character
    {
        private int state;

        protected Character(string name)
        {
            Name = name;
            BareHandDamage = new DamageDiceSpecification(1, 2);
            MaximumHitPoints = 50;
            HitPoints = MaximumHitPoints;
            State = CharacterState.Standing;
        }

        public string Name { get; }

        public int HitPoints { get; set; }

        public int MaximumHitPoints { get; set; }

        public DamageDiceSpecification BareHandDamage { get; set; }

        public Weapon? Weapon { get; set; }

        public CharacterState State
        {
            get => (CharacterState)Volatile.Read(ref state);
            set => Volatile.Write(ref state, (int)value);
        }

        public void EndFight()
        {
            State = CharacterState.Standing;
        }

        public bool TryStartFight()
        {
            return Interlocked.CompareExchange(
                ref state,
                (int)CharacterState.Fighting,
                (int)CharacterState.Standing) == (int)CharacterState.Standing;
        }
    }
}
