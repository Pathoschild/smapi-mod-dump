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
using System.Collections.Generic;

namespace BetterTrainLoot.Data
{
    public enum TRAINS
    {
        RANDOM_TRAIN = 0,
        JOJA_TRAIN = 1,
        COAL_TRAIN = 2,
        PASSENGER_TRAIN =3,
        UNKNOWN = 4,
        PRISON_TRAIN = 5,
        PRESENT_TRAIN = 6
    }

    public class TrainData 
    {
        public TRAINS TrainCarID { get; set; }

        public List<TrainTreasure> treasureList { get; set; }

        public TrainData(TRAINS id)
        {
            this.TrainCarID = id;
        }

        public void UpdateTrainLootChances(double todayLuck)
        {                        
            double itemBaseChance = 0.0;
            foreach (TrainTreasure item in this.treasureList)
            {
                if (item.Id != 434) // If not a Stardrop
                {
                    itemBaseChance = Game1.random.NextDouble() / 10.0;  // The bestcase is 10% (0.1)
                    itemBaseChance = itemBaseChance + (itemBaseChance * todayLuck);

                    item.Chance = itemBaseChance * (double)item.Rarity;
                }
                else
                {
                    itemBaseChance = Game1.random.NextDouble() / 100.0;  // The bestcase is 1% (0.01)
                    itemBaseChance = itemBaseChance + (itemBaseChance * todayLuck);

                    item.Chance = itemBaseChance * (double)item.Rarity;
                }               
            }           
        }

        internal bool HasItem(int id)
        {
            for (var i = 0; i<treasureList.Count; i++)
            {
                if (treasureList[i].Id == id)
                    return true;
            }

            return false;
        }
    }
}
