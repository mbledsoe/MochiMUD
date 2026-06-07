using MochiMud.WebApp.Combat;

namespace MochiMud.WebApp.Characters
{
    public class Weapon
    {
        public Weapon(string name, DamageDiceSpecification damageDice)
        {
            Name = name;
            DamageDice = damageDice;
        }

        public string Name { get; }

        public DamageDiceSpecification DamageDice { get; }
    }
}
