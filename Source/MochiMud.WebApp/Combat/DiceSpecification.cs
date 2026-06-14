namespace MochiMud.WebApp.Combat
{
    public record DiceSpecification(int DiceCount, int Sides)
    {
        public int Roll()
        {
            var total = 0;

            for (var i = 0; i < DiceCount; i++)
            {
                total += Random.Shared.Next(1, Sides + 1);
            }

            return total;
        }
    }
}
