/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TyoAtrosa/TreeShaker
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TreeShaker
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += this.DayStarted;
        }

        private void DayStarted()
        {
            foreach (Farm location in Game1.Locations.OfType<Farm>())
            {
                foreach (Tree tree in location.terrainFeatures.Values.OfType<Tree>())
                {
                    if (!tree.hasSeed)
                        continue;

                    bool added = false;
                    switch (tree.Type.Value)
                    {
                        case 1:
                            added = Game1.player.addItemToInventoryBool(309);
                            break;
                        case 2:
                            added = Game1.player.addItemToInventoryBool(310);
                            break;
                        case 3:
                            added = Game1.player.addItemToInventoryBool(311);
                            break;
                        case 6:
                            added = Game1.player.addItemToInventoryBool(88);
                            break;
                    }

                    if (added)
                        Tree.hasSeed = false;
                }
            }
        }
    }
}