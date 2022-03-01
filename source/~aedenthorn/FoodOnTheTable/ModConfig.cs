/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace FoodOnTheTable
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MinutesToHungry { get; set; } = 300;
        public float MoveToFoodChance { get; set; } = 0.1f;
        public float MaxDistanceToEat { get; set; } = 3f;
        public float PointsMult { get; set; } = 0.5f;
    }
}
