/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace WaterYourCrops
{
    partial class ModEntry
    {
        public static bool HasCan()
        {
            Item item = Game1.player.CurrentItem;
            if (item is not null)
            {
                //Log($"{Game1.player} has a watering can", debugOnly: true);
                return (item.Name.Contains("Watering Can"));
            }
            //Log($"{Game1.player} does not have a watering can", debugOnly: true);
            return false;
        }
    }
}
