using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Records the state of Game1.player.UsingTool during the previous update tick.</summary>
        static bool UsingToolOnPreviousTick = false;

        private void PlayerUsedTool(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.UsingTool != UsingToolOnPreviousTick) //if UsingTool has changed since the previous tick
            {
                UsingToolOnPreviousTick = Game1.player.UsingTool; //update the "last tick" value

                if (Game1.player.UsingTool) //if the player started using a tool on this tick
                {
                    Vector2 targetTile = new Vector2((int)(Game1.player.GetToolLocation().X / Game1.tileSize), (int)(Game1.player.GetToolLocation().Y / Game1.tileSize)); //get the tile on which the tool was used
                    Rectangle targetBox = new Rectangle(((int)targetTile.X) * 64, ((int)targetTile.Y) * 64, Game1.tileSize, Game1.tileSize); //get a rectangle representing the target tile
                    var ltf = Game1.currentLocation.largeTerrainFeatures; //alias the current location's large terrain feature list

                    for (int x = ltf.Count - 1; x >= 0; x--) //for each large terrain feature at the current location (looping backward for removal purposes)
                    {
                        if (ltf[x] is LargeResourceClump clump) //if this is a large resource clump
                        {
                            if (clump.getBoundingBox(clump.tilePosition.Value).Intersects(targetBox)) //if this was hit by the tool
                            {
                                //NOTE: the reflection below prevents a non-fatal error when an Axe or Pickaxe is used for the first time to hit a LargeResourceClump containing a GiantCrop;
                                //      this is due to the "lastUser" field being null during the first use of those tool subclasses
                                IReflectedField<Farmer> lastUser = Utility.Helper.Reflection.GetField<Farmer>(Game1.player.CurrentTool, "lastUser", false); //get the protected "lastUser" field of this tool
                                lastUser?.SetValue(Game1.player); //set "lastUser" to the player

                                bool destroyed = clump.Clump.Value.performToolAction(Game1.player.CurrentTool, 0, targetTile, Game1.currentLocation); //make the inner ResourceClump react to being hit by the tool

                                if (destroyed) //if this clump was "destroyed" by this tool hit
                                {
                                    ltf.RemoveAt(x); //remove it from the list
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
