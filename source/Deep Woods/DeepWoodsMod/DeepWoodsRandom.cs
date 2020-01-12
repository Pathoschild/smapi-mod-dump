using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Linq;

namespace DeepWoodsMod
{
    public class DeepWoodsRandom
    {
        private const int MAGIC_SALT = 854574563;

        private readonly DeepWoods deepWoods;
        private readonly int seed;
        private readonly Random random;
        private readonly Random masterRandom;
        // private int masterModeCounter;

        public class LuckValue
        {
            public int BadLuck { get; set; }
            public int Neutral { get; set; }
            public int GoodLuck { get; set; }

            // For JSON serialization
            public LuckValue() { }

            public LuckValue(int badLuck, int goodLuck)
            {
                BadLuck = badLuck;
                GoodLuck = goodLuck;
                Neutral = (BadLuck + GoodLuck) / 2;
            }

            public LuckValue(int badLuck, int neutral, int goodLuck)
            {
                BadLuck = badLuck;
                Neutral = neutral;
                GoodLuck = goodLuck;
            }
        }

        public class LuckRange
        {
            public LuckValue LowerBound { get; set; }
            public LuckValue UpperBound { get; set; }

            // For JSON serialization
            public LuckRange() { }

            public LuckRange(LuckValue lowerBound, LuckValue upperBound)
            {
                LowerBound = lowerBound;
                UpperBound = upperBound;
            }
        }

        public class WeightedValue<T>
        {
            public T Value { get; set; }
            public LuckValue Weight { get; set; }

            // For JSON serialization
            public WeightedValue() { }

            public WeightedValue(T value, LuckValue weight)
            {
                Value = value;
                Weight = weight;
            }

            public WeightedValue(T value, int weight)
            {
                Value = value;
                Weight = new LuckValue(weight, weight, weight);
            }
        }

        public class WeightedInt : WeightedValue<int>
        {
            // For JSON serialization
            public WeightedInt() { }

            public WeightedInt(int value, LuckValue weight)
                : base(value, weight)
            {
            }

            public WeightedInt(int value, int weight)
                : base(value, weight)
            {
            }
        }

        public class Chance
        {
            public class LevelModifier
            {
                public class Bound
                {
                    public int Level { get; set; }
                    public int Modifier { get; set; }
                }

                public readonly static LevelModifier ZERO = new LevelModifier(0, 0, 0, 0);

                public Bound Min { get; set; }
                public Bound Max { get; set; }

                // For JSON serialization
                public LevelModifier() { }

                public LevelModifier(int minLevel, int minLevelModifier, int maxLevel, int maxLevelModifier)
                {
                    Min = new Bound()
                    {
                        Level = minLevel,
                        Modifier = minLevelModifier
                    };
                    Max = new Bound()
                    {
                        Level = maxLevel,
                        Modifier = maxLevelModifier
                    };
                }
            }

            public const int PROCENT = 100;
            public const int PROMILLE = 1000;

            public readonly static Chance FIFTY_FIFTY = new Chance(50);

            public LuckValue Value { get; set; }
            public int Range { get; set; }
            public LevelModifier RangeLevelModifier { get; set; }

            // For JSON serialization
            public Chance() { }

            public Chance(LuckValue value, int range = PROCENT, LevelModifier levelModifier = null)
            {
                Value = value;
                Range = range;
                RangeLevelModifier = levelModifier ?? LevelModifier.ZERO;
            }

            public Chance(int value, int range = PROCENT, LevelModifier levelModifier = null)
            {
                Value = new LuckValue(value, value, value);
                Range = range;
                RangeLevelModifier = levelModifier ?? LevelModifier.ZERO;
            }
        }

        public DeepWoodsRandom(DeepWoods deepWoods, int seed)
        {
            this.deepWoods = deepWoods;
            this.seed = seed;
            this.random = new Random(this.seed);
            this.masterRandom = new Random(this.seed ^ Game1.random.Next());
            // this.masterModeCounter = 0;
        }

        public bool IsInMasterMode()
        {
            return false;// this.masterModeCounter > 0;
        }

        public int GetSeed()
        {
            return this.seed;
        }

        public static int CalculateSeed(int level, DeepWoodsEnterExit.EnterDirection enterDir, int? salt)
        {
            if (level == 1)
            {
                // This is the "root" DeepWoods level, always use UniqueMultiplayerID as seed.
                // This makes sure the first level stays the same for the entire game, but still be different for each unique game experience.
                return GetHashFromUniqueMultiplayerID() ^ MAGIC_SALT;
            }
            else
            {
                // Calculate seed from multiplayer ID, DeepWoods level, enter direction and time since start.
                // This makes sure the seed is the same for all players entering the same DeepWoods level during the same game hour,
                // but still makes it unique for each game and pseudorandom enough for players to not be able to reasonably predict the woods.
                return GetHashFromUniqueMultiplayerID() ^ UniformAnyInt(level) ^ UniformAnyInt((int)enterDir) ^ UniformAnyInt(MinutesSinceStart()) ^ ((salt.HasValue && salt.Value != 0) ? salt.Value : MAGIC_SALT);
            }
        }

        private static int UniformAnyInt(int x)
        {
            // From https://stackoverflow.com/a/12996028/9199167
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = (x >> 16) ^ x;
            return x;
        }

        private static int GetHashFromUniqueMultiplayerID()
        {
            ulong uniqueMultiplayerID = Game1.uniqueIDForThisGame; //Game1.MasterPlayer.UniqueMultiplayerID;
            return UniformAnyInt((int)((uniqueMultiplayerID >> 32) ^ uniqueMultiplayerID));
        }

        private static int MinutesSinceStart()
        {
            return Game1.timeOfDay + SDate.Now().DaysSinceStart * 10000;
        }

        private Random GetRandom()
        {
            if (this.IsInMasterMode())
            {
                return this.masterRandom;
            }
            else
            {
                return this.random;
            }
        }

        private int GetAbsoluteLuckValue(LuckValue value)
        {
            // Daily luck in range from -100 to 100:
            int dailyLuck = Math.Min(100, Math.Max(-100, (int)((Game1.player.team.sharedDailyLuck.Value / 0.12) * 100.0)));

            // Player luck in range from 0 to 100:
            int playerLuck = Math.Min(100, Math.Max(0, deepWoods.GetLuckLevel() * 10));

            // Total luck in range from -100 to 100:
            int totalLuck = Math.Min(100, Math.Max(-100, (dailyLuck + playerLuck) / 2));

            if (totalLuck < 0)
            {
                int badLuckFactor = -totalLuck;
                int neutralFactor = 100 - badLuckFactor;
                return ((value.BadLuck * badLuckFactor) + (value.Neutral * neutralFactor)) / 100;
            }
            else
            {
                int goodLuckFactor = totalLuck;
                int neutralFactor = 100 - goodLuckFactor;
                return ((value.GoodLuck * goodLuckFactor) + (value.Neutral * neutralFactor)) / 100;
            }
        }

        private int GetAbsoluteRange(int range, Chance.LevelModifier rangeLevelModifier)
        {
            if (deepWoods.level.Value >= rangeLevelModifier.Max.Level)
            {
                range += rangeLevelModifier.Max.Modifier;
            }
            else if (deepWoods.level.Value > rangeLevelModifier.Min.Level)
            {
                double totalDelta = rangeLevelModifier.Max.Level - rangeLevelModifier.Min.Level;
                double delta = deepWoods.level.Value - rangeLevelModifier.Min.Level;
                double maxFactor = delta / totalDelta;
                double minFactor = 1.0 - maxFactor;
                int modifier = (int)(rangeLevelModifier.Min.Modifier * minFactor + rangeLevelModifier.Max.Modifier * maxFactor);
                range += Math.Min(rangeLevelModifier.Max.Modifier, Math.Max(rangeLevelModifier.Min.Modifier, modifier));
            }
            else if (deepWoods.level.Value == rangeLevelModifier.Min.Level)
            {
                range += rangeLevelModifier.Min.Modifier;
            }
            // else deepWoods.level.Value < rangeLevelModifier.Min.Level

            return range;
        }

        public int GetRandomValue()
        {
            return GetRandom().Next();
        }

        public int GetRandomValue(int min, int max)
        {
            return GetRandom().Next(min, max);
        }

        public bool CheckChance(Chance chance)
        {
            return GetRandomValue(0, GetAbsoluteRange(chance.Range, chance.RangeLevelModifier)) < GetAbsoluteLuckValue(chance.Value);
        }

        public int GetRandomValue(LuckRange range)
        {
            return GetRandomValue(GetAbsoluteLuckValue(range.LowerBound), GetAbsoluteLuckValue(range.UpperBound));
        }

        public T GetRandomValue<T>(T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("values is null or empty");

            return values[GetRandomValue(0, values.Length)];
        }

        public int GetRandomValue(WeightedInt[] values)
        {
            return GetRandomValue<int>(values);
        }

        public T GetRandomValue<T>(WeightedValue<T>[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("values is null or empty");

            int total = values.Sum(wv => GetAbsoluteLuckValue(wv.Weight));
            int n = GetRandomValue(0, total);

            int sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += GetAbsoluteLuckValue(values[i].Weight);
                if (n < sum)
                {
                    return values[i].Value;
                }
            }

            throw new InvalidOperationException("Impossible to get here.");
        }

        /*
        public void EnterMasterMode()
        {
            // Master Mode is used when generating interactive content (monsters, terrain features, loot etc.)
            // These things are only generated by the server (while the map itself is generated on every client, hence the shared seed),
            // so when in master mode, we use Game1.random instead of our own random.
            // This ensures server-side only generation doesn't mess with shared generation (as the shared random stays in sync).
            this.masterModeCounter++;
        }

        public void LeaveMasterMode()
        {
            this.masterModeCounter--;
        }
        */
    }
}
