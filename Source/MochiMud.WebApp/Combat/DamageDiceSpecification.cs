namespace MochiMud.WebApp.Combat
{
    public record DamageDiceSpecification(int DiceCount, int Sides)
    {
        public int Roll()
        {
            var damage = 0;

            for (var i = 0; i < DiceCount; i++)
            {
                damage += Random.Shared.Next(1, Sides + 1);
            }

            return damage;
        }
    }
}
