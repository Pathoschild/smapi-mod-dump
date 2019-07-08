using BetterGarbageCans.Framework;
using StardewValley;

namespace BetterGarbageCans.Data
{
    public class TrashTreasure : IWeighted
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public double Chance { get; set; }

        public bool Enabled { get; set; }
                
        public int MinAmount { get; set; }

        public int MaxAmount { get; set; }

        public int AvailableStartTime { get; set; }
        
        public int AvailableEndTime { get; set; }

        public TrashTreasure(int id, string name, double chance, int minAmount = 1, int maxAmount = 1, int startTime = 600, int endTime = 2600, bool enabled = true)
        {
            this.Id = id;
            this.Name = name;
            this.Chance = chance;
            this.Enabled = enabled;
            this.MinAmount = minAmount;
            this.MaxAmount = maxAmount;
            this.AvailableStartTime = startTime;
            this.AvailableEndTime = endTime;
        }

        public bool IsValid()
        {
            int time = Game1.timeOfDay;
            return  time >= this.AvailableStartTime && time <= this.AvailableEndTime;
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
