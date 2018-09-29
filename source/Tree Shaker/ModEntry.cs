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