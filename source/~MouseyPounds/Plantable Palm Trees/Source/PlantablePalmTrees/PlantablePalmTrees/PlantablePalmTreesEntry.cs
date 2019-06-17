using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace PlantablePalmTrees
{
    public class PlantablePalmTreesEntry : Mod
    {
        private PlantablePalmTreesConfig Config;
        const int PalmTreeSeed = 88; // This is the objectID of Coconut in unmodified game
        const int OakTreeSeed = 309; // This is the objectID of Acorn in unmodified game
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<PlantablePalmTreesConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.RenderingHud += Display_RenderingHud;

        }

        // Basic planting logic is done in a handler for right-clicking
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.activeClickableMenu == null &&
                Game1.player.CurrentItem != null &&
                Game1.player.CurrentItem.canBeGivenAsGift() &&
                Game1.player.CurrentItem.ParentSheetIndex == PalmTreeSeed &&
                (e.Button.IsActionButton() || e.Button.IsUseToolButton()) &&
                (!this.Config.Use_Modifier_Key || Helper.Input.IsDown(this.Config.Modifier_Key)))
            {
                //this.Monitor.Log("Passed basic checks", LogLevel.Trace);
                GameLocation loc = Game1.currentLocation;
                Vector2 tile = e.Cursor.GrabTile;
                loc.terrainFeatures.TryGetValue(tile, out TerrainFeature terr);
                if (!this.Config.Require_Tilled_Soil || (terr != null && terr is HoeDirt && !((HoeDirt)terr).state.Equals(2)))
                {
                    // We've reached a point where we are ready to plant. Rather than duplicating a ton of vanilla code,
                    // we are going to see if the game would let us plant an oak tree here. We run two different checks
                    // because we aren't sure exactly which should be used, but if they both pass, we plant the oak and 
                    // then change it to a palm.
                    //this.Monitor.Log("Dirt is properly tilled (or we don't care)", LogLevel.Trace);
                    StardewValley.Object fakeSeed = new StardewValley.Object(OakTreeSeed, 1);

                    if (Utility.playerCanPlaceItemHere(loc, fakeSeed, (int)tile.X*64, (int)tile.Y*64, Game1.player) &&
                        fakeSeed.canBePlacedHere(loc, tile))
                    {
                        //this.Monitor.Log("Acorn could be planted", LogLevel.Trace);
                        if (fakeSeed.placementAction(loc, (int)tile.X*64, (int)tile.Y*64))
                        {
                            // Successfully placed an oak tree. Quick, let's change it
                            //this.Monitor.Log("Acorn WAS planted and will now be changed.", LogLevel.Trace);
                            loc.terrainFeatures.Remove(tile);
                            loc.terrainFeatures.Add(tile, new Tree(Tree.palmTree, 0));
                            Game1.player.reduceActiveItemByOne();
                            // Since we've handled the click, we prevent it from passing on to the game
                            Helper.Input.Suppress(e.Button);
                        }
                    }
                }
            }
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.activeClickableMenu == null &&
                Game1.player.CurrentItem != null &&
                Game1.player.CurrentItem.canBeGivenAsGift() &&
                Game1.player.CurrentItem.ParentSheetIndex == PalmTreeSeed &&
                this.Config.Show_Placement_Icon &&
                (!this.Config.Use_Modifier_Key || Helper.Input.IsDown(this.Config.Modifier_Key)))
            {
                // Again, let's pretend to be an acorn
                StardewValley.Object fakeSeed = new StardewValley.Object(OakTreeSeed, 1);
                fakeSeed.drawPlacementBounds(e.SpriteBatch, Game1.currentLocation);
            }
        }

    }
}
