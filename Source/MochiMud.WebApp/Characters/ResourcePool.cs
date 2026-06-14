namespace MochiMud.WebApp.Characters
{
    public sealed record ResourcePool
    {
        public ResourcePool(int current, int maximum)
        {
            if (maximum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), maximum, "Maximum resource value cannot be negative.");
            }

            if (current is < 0 || current > maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(current),
                    current,
                    "Current resource value must be between zero and maximum.");
            }

            Current = current;
            Maximum = maximum;
        }

        public int Current { get; }

        public int Maximum { get; }

        public static ResourcePool Empty { get; } = new(0, 0);

        public static ResourcePool Full(int maximum)
        {
            return new ResourcePool(maximum, maximum);
        }

        public bool HasAtLeast(int amount)
        {
            return Current >= amount;
        }

        public ResourcePool Spend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Spent amount cannot be negative.");
            }

            if (!HasAtLeast(amount))
            {
                throw new InvalidOperationException("Not enough resource is available.");
            }

            return new ResourcePool(Current - amount, Maximum);
        }

        public ResourcePool Reduce(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Reduced amount cannot be negative.");
            }

            return new ResourcePool(Math.Max(0, Current - amount), Maximum);
        }

        public ResourcePool Restore(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Restored amount cannot be negative.");
            }

            return new ResourcePool(Math.Min(Maximum, Current + amount), Maximum);
        }

        public ResourcePool RestoreToMaximum()
        {
            return new ResourcePool(Maximum, Maximum);
        }
    }
}
