using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Combat;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Players
{
    public class Player : Character
    {
        public const int MinimumLevel = 1;
        public const int MaximumLevel = 50;
        public const int MageStartingMana = 20;
        public const int ClericStartingMana = 20;

        private int level;

        public Player(
            Guid id,
            string name,
            PlayerClass playerClass,
            int level = MinimumLevel,
            int experiencePoints = 0,
            ResourcePool? mana = null,
            bool autoExits = false)
            : base(name)
        {
            Id = id;
            Class = playerClass;
            Level = level;
            ExperiencePoints = experiencePoints;
            Mana = mana ?? GetStartingMana(playerClass);
            AutoExits = autoExits;
            CurrentRoomId = WorldConstants.DefaultStartRoomId;
            Weapon = new Weapon("A rusty epee", new DiceSpecification(1, 6));
        }

        public Player(string name, PlayerClass playerClass)
            : this(Guid.NewGuid(), name, playerClass)
        {
        }

        public Guid Id { get; }

        public PlayerClass Class { get; }

        public int Level
        {
            get => level;
            set
            {
                if (value is < MinimumLevel or > MaximumLevel)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"Player level must be between {MinimumLevel} and {MaximumLevel}.");
                }

                level = value;
            }
        }

        public int ExperiencePoints { get; set; }

        public ResourcePool Mana { get; set; }

        public bool AutoExits { get; set; }

        public Guid CurrentRoomId { get; set; }

        public int AddExperience(int amount)
        {
            if (amount <= 0 || Level >= MaximumLevel)
            {
                if (Level >= MaximumLevel)
                {
                    ExperiencePoints = 0;
                }

                return 0;
            }

            ExperiencePoints += amount;

            var levelsGained = 0;

            while (Level < MaximumLevel &&
                   ExperiencePoints >= ExperienceTable.XpToNextLevel(Level))
            {
                ExperiencePoints -= ExperienceTable.XpToNextLevel(Level);
                Level += 1;
                levelsGained += 1;
            }

            if (Level >= MaximumLevel)
            {
                ExperiencePoints = 0;
            }

            return levelsGained;
        }

        public static Player FromData(PlayerData data)
        {
            return new Player(
                data.Id,
                data.Name,
                data.Class,
                data.Level,
                data.ExperiencePoints,
                data.Mana,
                data.AutoExits)
            {
                HitPoints = data.HitPoints,
            };
        }

        private static ResourcePool GetStartingMana(PlayerClass playerClass)
        {
            return playerClass switch
            {
                PlayerClass.Cleric => ResourcePool.Full(ClericStartingMana),
                PlayerClass.Mage => ResourcePool.Full(MageStartingMana),
                _ => ResourcePool.Empty,
            };
        }
    }
}
