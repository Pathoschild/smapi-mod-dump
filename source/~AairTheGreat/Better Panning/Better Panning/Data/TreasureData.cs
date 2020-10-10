/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;

namespace BetterPanning.Data
{
//    [JsonDescribe]
    public class TreasureData : IWeighted
    {

  //      [Description("ID of the first (or only) item")]
        public int Id { get; set; }

        public string Name { get; set; }

        //       [Description("Weighted chance this range of items will get chosen")]
        public double Chance { get; set; }

//        [Description("The minimum amount of this that can be found in one slot")]
        public int MinAmount { get; set; }

//        [Description("The maximum amount of this that can be found in one slot")]
        public int MaxAmount { get; set; }

//        [Description("Whether this can be found multiple times in one chest")]
        public bool AllowDuplicates { get; set; }

        public bool Enabled { get; set; }
        public TreasureData(int id, string name, double chance, int minAmount = 1, int maxAmount = 1, bool allowDuplicates = true, bool enabled = true)
        {
            this.Id = id;
            this.Name = name;            
            this.Chance = chance;
            this.MinAmount = Math.Max(1, minAmount);
            this.MaxAmount = Math.Max(this.MinAmount, maxAmount);
            this.AllowDuplicates = allowDuplicates;
            this.Enabled = enabled;
        }

        public bool IsValid(Farmer who)
        {
            return true; 
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