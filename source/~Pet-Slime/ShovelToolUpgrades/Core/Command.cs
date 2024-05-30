/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonShared;
using MoonShared.Command;
using StardewValley;
using StardewValley.Tools;

namespace ShovelToolUpgrades
{
    [CommandClass]
    public class Command
    {
        [CommandMethod("Add a shovel to the player inventory")]
        public static void GiveShovel(int level = 0)
        {
            Game1.player.addItemToInventory(new UpgradeableShovel(level));
        }

        [CommandMethod("Remove a shovel from the player inventory")]
        public static void RemoveShovel()
        {
            Item shovel = Game1.player.getToolFromName("Shovel");
            if (shovel is not null)
            {
                Game1.player.removeItemFromInventory(shovel);
            }
        }
    }
}
