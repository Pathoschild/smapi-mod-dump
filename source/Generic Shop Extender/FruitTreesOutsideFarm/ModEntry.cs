using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley.Menus;
using System;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Netcode;
using StardewValley.Network;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace FruitTreesOutsideFarm
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += PlayerInputEvent;
            SaveEvents.BeforeSave += EndOfDayEvent;
        }

        private void PlayerInputEvent(object sender, EventArgsInput eventArguments)
        {
            if ((eventArguments.IsActionButton || eventArguments.IsUseToolButton))
            {
                if (Game1.player.ActiveObject != null && !(Game1.player.ActiveObject is Tool) && Game1.player.ActiveObject.Name.Contains("Sapling"))
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

        private void GrowTrees()
        {
            foreach (GameLocation locationIterator in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, NetRef<TerrainFeature>> potentialFruitTree in locationIterator.terrainFeatures.FieldPairs)
                {
                    if (potentialFruitTree.Value.Value is FruitTree)
                    {
                        FruitTree fruitTree = potentialFruitTree.Value.Value as FruitTree;
                        Boolean justBloomed = false;
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

        private void EndOfDayEvent(object sender, EventArgs eventArguments)
        {
            GrowTrees();
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
                if (!(currentLocation.terrainFeatures[proposedTreeLocation] is HoeDirt) || (currentLocation.terrainFeatures[proposedTreeLocation] as HoeDirt).crop != null)
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
            return;
        }
    }
}