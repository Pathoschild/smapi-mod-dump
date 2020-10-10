/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/IrregularPorygon/SMAPIGenericShopMod
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FruitTreesOutsideFarm
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton() || e.Button.IsUseToolButton())
            {
                if (Game1.player.ActiveObject != null && Game1.player.ActiveObject.Name.Contains("Sapling"))
                {
                    Vector2 plantingPosition = GetPlantingPosition();
                    if (!Game1.eventUp || Game1.isFestival())
                    {
                        if (Game1.tryToCheckAt(plantingPosition, Game1.player) || Game1.player.isRidingHorse() || !Game1.player.canMove)
                        {
                            return;
                        }
                        if (Game1.player.ActiveObject != null && !(Game1.player.ActiveObject is Furniture))
                        {
                            int positionToPlantX = (int)plantingPosition.X * Game1.tileSize + Game1.tileSize / 2;
                            int positionToPlantY = (int)plantingPosition.Y * Game1.tileSize + Game1.tileSize / 2;
                            TryToPlantTree(Game1.currentLocation, Game1.player.ActiveObject, positionToPlantX, positionToPlantY);
                        }
                    }
                }
            }
            /*
            else if (eventArguments.Button == SButton.K)
            {
                GrowTrees();
            }
            */
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            GrowTrees();
        }

        private void GrowTrees()
        {
            foreach (GameLocation locationIterator in Game1.locations)
            {
                foreach (var potentialFruitTree in locationIterator.terrainFeatures.Pairs)
                {
                    if (potentialFruitTree.Value is FruitTree tree)
                    {
                        FruitTree fruitTree = tree;
                        bool justBloomed = false;
                        if (fruitTree.daysUntilMature.Value > 28)
                        {
                            fruitTree.daysUntilMature.Value = 28;
                        }
                        fruitTree.daysUntilMature.Value--;

                        if (fruitTree.daysUntilMature.Value % 7 == 0)
                        {
                            fruitTree.growthStage.Value++;
                            if (fruitTree.growthStage.Value == 4)
                            {
                                justBloomed = true;
                            }
                        }
                        if (fruitTree.growthStage.Value > 4) { fruitTree.growthStage.Value = 4; }

                        if (justBloomed && (locationIterator.Name.ToLower().Contains("greenhouse") || Game1.currentSeason.Equals(fruitTree.fruitSeason.Value)))
                        {
                            fruitTree.fruitsOnTree.Value++;
                        }

                    }
                }
            }
        }

        private Vector2 GetPlantingPosition()
        {
            Vector2 plantingPosition = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
            if (Game1.mouseCursorTransparency == 0.0 || !Game1.wasMouseVisibleThisFrame || !Game1.lastCursorMotionWasMouse && (Game1.player.ActiveObject == null || !Game1.player.ActiveObject.isPlaceable() && Game1.player.ActiveObject.Category != -74))
            {
                plantingPosition = Game1.player.GetGrabTile();
                if (plantingPosition.Equals(Game1.player.getTileLocation()))
                {
                    plantingPosition = Utility.getTranslatedVector2(plantingPosition, Game1.player.facingDirection, 1f);
                }
            }
            if (!Utility.tileWithinRadiusOfPlayer((int)plantingPosition.X, (int)plantingPosition.Y, 1, Game1.player))
            {
                plantingPosition = Game1.player.GetGrabTile();
                if (plantingPosition.Equals(Game1.player.getTileLocation()) && Game1.isAnyGamePadButtonBeingPressed())
                {
                    plantingPosition = Utility.getTranslatedVector2(plantingPosition, Game1.player.facingDirection, 1f);
                }
            }

            return plantingPosition;
        }

        private void TryToPlantTree(GameLocation currentLocation, Item sapling, int x, int y)
        {
            if (Utility.playerCanPlaceItemHere(currentLocation, sapling, x, y, Game1.player))
            {
                ActuallyPlantTree(sapling, currentLocation, x, y);
            }
        }

        private void ActuallyPlantTree(Item sapling, GameLocation currentLocation, int x, int y)
        {
            Vector2 proposedTreeLocation = new Vector2(x / Game1.tileSize, y / Game1.tileSize);

            if (currentLocation.terrainFeatures.ContainsKey(proposedTreeLocation))
            {
                if (!(currentLocation.terrainFeatures[proposedTreeLocation] is HoeDirt) || ((HoeDirt)currentLocation.terrainFeatures[proposedTreeLocation]).crop != null)
                {
                    return;
                }
                currentLocation.terrainFeatures.Remove(proposedTreeLocation);
            }


            if ((currentLocation.doesTileHaveProperty((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "Diggable", "Back") != null ||
                 currentLocation.doesTileHavePropertyNoNull((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "Type", "Back").Equals("Grass")) &&
                !currentLocation.doesTileHavePropertyNoNull((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "NoSpawn", "Back").Equals("Tree") || currentLocation.Name.Equals("Greenhouse") &&
                (currentLocation.doesTileHaveProperty((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "Diggable", "Back") != null || currentLocation
                     .doesTileHavePropertyNoNull((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "Type", "Back").Equals("Stone")))
            {
                Game1.playSound("dirtyHit");
                DelayedAction.playSoundAfterDelay("coin", 100);
                currentLocation.terrainFeatures.Add(proposedTreeLocation, new FruitTree(sapling.ParentSheetIndex)
                {
                    GreenHouseTree = currentLocation.IsGreenhouse,
                    GreenHouseTileTree = currentLocation.doesTileHavePropertyNoNull((int)proposedTreeLocation.X, (int)proposedTreeLocation.Y, "Type", "Back").Equals("Stone")
                });
                Game1.player.reduceActiveItemByOne();
            }

            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068"));
        }
    }
}
