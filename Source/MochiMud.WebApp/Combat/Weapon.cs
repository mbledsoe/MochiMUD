namespace MochiMud.WebApp.Combat
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
