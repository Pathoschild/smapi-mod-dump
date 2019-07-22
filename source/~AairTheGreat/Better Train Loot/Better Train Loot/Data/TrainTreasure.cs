using BetterTrainLoot.Framework;
using StardewValley;

namespace BetterTrainLoot.Data
{
    public enum LOOT_RARITY
    {
        NONE = 0,
        ULTRA_RARE = 1,
        RARE = 2,     
        UNCOMMON = 6,
        COMMON = 10,
    }

    public class TrainTreasure : IWeighted
    {
        public int Id { get; set; }

        public string Name { get; set; }

        internal double Chance { get; set; }

        public bool Enabled { get; set; }

        public LOOT_RARITY Rarity {get; set;}

        public TrainTreasure(int id, string name, double chance, LOOT_RARITY rarity, bool enabled = true)
        {
            this.Id = id;
            this.Name = name;
            this.Chance = chance;
            this.Enabled = enabled;
            this.Rarity = rarity;            
        }

        public bool IsValid()
        {
            return this.Rarity != LOOT_RARITY.NONE;
        }

        public double GetWeight()
        {
            return this.Chance;
        }

        public bool GetEnabled()
        {
            return this.Enabled;
        }
    }
}
