/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirbShared.Command;
using StardewValley;

namespace RanchingToolUpgrades
{
    [CommandClass]
    public class Command
    {
        [CommandMethod("Add a pail to the player inventory")]
        public static void GivePail(int level = 0)
        {
            Game1.player.addItemToInventory(new UpgradeablePail(level));
        }

        [CommandMethod("Add shears to the player inventory")]
        public static void GiveShears(int level = 0)
        {
            Game1.player.addItemToInventory(new UpgradeableShears(level));
        }

        [CommandMethod("Remove a pail from the player inventory")]
        public static void RemovePail()
        {
            Item pail = Game1.player.getToolFromName("Pail");
            if (pail is not null)
            {
                Game1.player.removeItemFromInventory(pail);
            }
        }

        [CommandMethod("Remove shears from the player inventory")]
        public static void RemoveShears()
        {
            Item shears = Game1.player.getToolFromName("Shears");
            if (shears is not null)
            {
                Game1.player.removeItemFromInventory(shears);
            }
        }

        [CommandMethod("Add the original Milk Pail item to the player inventory")]
        public static void GiveOriginalPail()
        {
            Game1.player.addItemToInventory(new StardewValley.Tools.MilkPail());
        }

        [CommandMethod("Add the original Shears item to the player inventory")]
        public static void GiveOriginalShears()
        {
            Game1.player.addItemToInventory(new StardewValley.Tools.Shears());
        }
    }
}
