using MochiMud.WebApp.Combat;

namespace MochiMud.WebApp.Characters
{
    public class Weapon
    {
        public Weapon(string name, DiceSpecification damageDice)
        {
            Name = name;
            DamageDice = damageDice;
        }

        public string Name { get; }

        public DiceSpecification DamageDice { get; }
    }
}
