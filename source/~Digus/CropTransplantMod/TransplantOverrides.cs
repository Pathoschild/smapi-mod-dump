/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CropTransplantMod
{
    internal class TransplantOverrides
    {
        private static IModEvents Events => CropTransplantModEntry.Events;

        internal static Object RegularPotObject = new Object(Vector2.Zero, 62);
        internal static HeldIndoorPot CurrentHeldIndoorPot = null;
        internal static bool ShakeFlag = false;

        internal static List<int> MissingGrassTilesIndexes = new List<int>{152,420,421,422,423};

        /// <summary>
        /// Override to lift the pot back when an empty spot on the tool bar is selected.
        /// </summary>
        /// <param name="__result"></param>
        /// <returns></returns>
        public static bool PressUseToolButton( ref bool __result)
        {
            if (Game1.fadeToBlack)
                return false;
            Game1.player.toolPower = 0;
            Game1.player.toolHold = 0;
            if (Game1.player.CurrentTool == null && Game1.player.ActiveObject == null)
            {
                Vector2 key = key = Game1.currentCursorTile;
                if (!Game1.currentLocation.Objects.ContainsKey(key) ||
                    !(Game1.currentLocation.Objects[key] is IndoorPot)
                    || (!Utility.tileWithinRadiusOfPlayer((int)key.X, (int)key.Y, 1, Game1.player) && !DataLoader.ModConfig.EnableUnlimitedRangeToTransplant))
                {
                    key = Game1.player.GetToolLocation(false) / 64f;
                    key.X = (float)(int)key.X;
                    key.Y = (float)(int)key.Y;
                }
                
                if (Game1.currentLocation.Objects.ContainsKey(key))
                {
                    Object @object = Game1.currentLocation.Objects[key];
                    if (@object is IndoorPot pot)
                    {
                        pot.performRemoveAction(pot.TileLocation, Game1.currentLocation);
                        Game1.currentLocation.Objects.Remove(pot.TileLocation);
                        HoeDirt potHoeDirt = pot.hoeDirt.Value;
                        if (potHoeDirt.crop != null )
                        {
                            CurrentHeldIndoorPot = new HeldIndoorPot(pot.TileLocation);
                            HoeDirt holdenHoeDirt = CurrentHeldIndoorPot.hoeDirt.Value;
                            holdenHoeDirt.crop = potHoeDirt.crop;
                            holdenHoeDirt.fertilizer.Value = potHoeDirt.fertilizer.Value;
                            TransplantController.ShakeCrop(holdenHoeDirt, pot.TileLocation);
                            Game1.player.Stamina -= ((float)DataLoader.ModConfig.CropTransplantEnergyCost - (float)Game1.player.FarmingLevel * DataLoader.ModConfig.CropTransplantEnergyCost / 20f);
                            Game1.player.ActiveObject = CurrentHeldIndoorPot;
                            Events.GameLoop.UpdateTicked += OnUpdateTicked;
                        }
                        else if (pot.bush.Value is Bush bush)
                        {
                            CurrentHeldIndoorPot = new HeldIndoorPot(pot.TileLocation);
                            CurrentHeldIndoorPot.bush.Value = bush;
                            Bush holdenBush = CurrentHeldIndoorPot.bush.Value;
                            TransplantController.ShakeBush(holdenBush);
                            Game1.player.Stamina -= ((float)DataLoader.ModConfig.CropTransplantEnergyCost - (float)Game1.player.FarmingLevel * DataLoader.ModConfig.CropTransplantEnergyCost / 20f);
                            Game1.player.ActiveObject = CurrentHeldIndoorPot;
                            Events.GameLoop.UpdateTicked += OnUpdateTicked;
                        }
                        else
                        {
                            Game1.player.ActiveObject = (Object)RegularPotObject.getOne();
                        }
                        
                        __result = true;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Override to ajust the original tool tip for the pot.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="item"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        public static bool PlayerCanPlaceItemHere(ref GameLocation location, Item item, int x, int y, ref Farmer f, ref bool __result)
        {
            if (Utility.isPlacementForbiddenHere(location))
            {
                return true;
            }
            if (item != null && item is Object object1 && TransplantController.IsGardenPot(object1) && object1.Stack == 1)
            {
                if (
                    (Game1.eventUp || f.bathingClothes.Value || f.onBridge.Value)
                    || (!Utility.withinRadiusOfPlayer(x, y, 1, f)
                        && (!Utility.withinRadiusOfPlayer(x, y, 2, f) || !Game1.isAnyGamePadButtonBeingPressed() || Game1.mouseCursorTransparency != 0f) 
                        && !DataLoader.ModConfig.EnableUnlimitedRangeToTransplant)
                ) return true;

                Vector2 tileLocation = new Vector2((float)(x / 64), (float)(y / 64));
                if (!(object1 is HeldIndoorPot heldPot))
                {
                    if (location.terrainFeatures.ContainsKey(tileLocation) && !location.objects.ContainsKey(tileLocation))
                    {
                        if (TransplantController.CanTransplantTerrainFeature(location.terrainFeatures[tileLocation]))
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
                else
                {
                    int tileLocationX = (int)tileLocation.X;
                    int tileLocationY = (int)tileLocation.Y;
                    if (heldPot.Tree != null)
                    {
                        if(!location.terrainFeatures.ContainsKey(tileLocation) && !location.objects.ContainsKey(tileLocation)
                           && (heldPot.Tree.isPassable() || !TransplantController.IntersectWithFarmer(location, x, y))
                           && location.doesTileHaveProperty(tileLocationX, tileLocationY, "Water", "Back") == null
                           && !location.isTileOccupiedForPlacement(tileLocation, new Object(heldPot.Tree.GetSeedSaplingIndex(),1)))
                        {
                            if (heldPot.Tree is Tree tree)
                            {
                                if (TransplantController.IsTreeNoSpawnTile(location,tileLocationX,tileLocationY)
                                    || (!location.isTileLocationOpen(new Location(x, y)))
                                    || (location.doesTileHaveProperty(tileLocationX, tileLocationY, "Diggable", "Back") == null && !(location is Farm) && !location.CanPlantTreesHere(tree.GetSeedIndex(), tileLocationX, tileLocationY) && !DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType))
                                {
                                    __result = false;
                                    return false;
                                }

                                __result = true;
                                return false;
                            }
                            else if (heldPot.Tree is FruitTree fruitTree)
                            {
                                if (
                                    (!TransplantController.IsNextToOtherTrees(location,x,y) || DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree)
                                    && 
                                    !(
                                        fruitTree.growthStage.Value < FruitTree.treeStage 
                                        && FruitTree.IsGrowthBlocked(tileLocation, location)
                                        && !DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth
                                    )
                                    && 
                                    (
                                        location is Farm 
                                        || location.CanPlantTreesHere(fruitTree.GetSaplingIndex(), tileLocationX, tileLocationY) 
                                        || DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm
                                    )
                                    && 
                                        TransplantController.IsValidTileForFruitTree(location,fruitTree.GetSaplingIndex(),tileLocationX,tileLocationY)
                                )
                                {
                                    __result = true;
                                    return false;
                                }
                            }
                        }
                        __result = false;
                        return false;
                    }
                    else if (heldPot.bush.Value != null)
                    {
                        if (!location.terrainFeatures.ContainsKey(tileLocation) && !location.objects.ContainsKey(tileLocation)
                            && !TransplantController.IntersectWithFarmer(location, x, y)
                            && location.doesTileHaveProperty(tileLocationX, tileLocationY, "Water", "Back") == null
                            && !location.isTileOccupiedForPlacement(tileLocation)
                            && (location is Farm || location.CanPlantTreesHere(heldPot.bush.Value.GetSaplingIndex(), tileLocationX, tileLocationY) || DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm)
                            && TransplantController.IsValidTileForBush(location, tileLocationX, tileLocationY))
                        {
                            __result = true;
                            return false;
                        }
                    }
                    else if (heldPot.hoeDirt.Value.crop != null)
                    {
                        if (location.terrainFeatures.ContainsKey(tileLocation) && !location.objects.ContainsKey(tileLocation)
                            && location.terrainFeatures[tileLocation] is HoeDirt hoeDirt
                            && (!heldPot.hoeDirt.Value.crop.raisedSeeds.Value || !TransplantController.IntersectWithFarmer(location, x, y))
                            && hoeDirt.crop == null)
                        {
                            __result = true;
                            return false;
                        }
                    }
                    if (DataLoader.ModConfig.EnableUnlimitedRangeToTransplant && (heldPot.bush.Value != null || heldPot.hoeDirt.Value.crop != null))
                    {
                        if (heldPot.canBePlacedHere(location, tileLocation))
                        {
                            foreach (Farmer farmer in location.farmers)
                            {
                                if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64)))
                                {
                                    return false;
                                }
                            }
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Override to not show the harvest tooltip while holding the pot.
        /// Override to change the tooltip when grabbing the pot from the ground. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="who"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        public static bool CanGrabSomethingFromHere(int x, int y, ref Farmer who, ref bool __result)
        {
            if (Game1.currentLocation == null)
                return false;
            Vector2 index = new Vector2((float)(x / 64), (float)(y / 64));
            Object activeObject = who.ActiveObject;
            if (activeObject is IndoorPot || TransplantController.IsGardenPot(activeObject))
            {
                __result = false;
                return false;
            } else if (who.CurrentTool == null && activeObject == null && Game1.currentLocation.Objects.ContainsKey(index) && Game1.currentLocation.Objects[index] is IndoorPot pot)
            {
                Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int)index.X, (int)index.Y, 1, who) && !DataLoader.ModConfig.EnableUnlimitedRangeToTransplant ? 0.5f : 1f;
                Game1.mouseCursor = 2;
                __result = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Override to let you lift trees and bushes
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="tileLocation"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static void TreeOrBushPerformUseAction(ref bool __result, Vector2 tileLocation, GameLocation location)
        {
            if (__result == true
                && TransplantController.IsGardenPot(Game1.player.ActiveObject)
                && location.terrainFeatures.ContainsKey(tileLocation) 
                && (location.terrainFeatures[tileLocation] is Tree || location.terrainFeatures[tileLocation] is FruitTree || location.terrainFeatures[tileLocation] is Bush))
            {
                __result = false;
            }
        }

        /// <summary>
        /// Draw dirty tile if tree over stone floor.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tileLocation"></param>
        public static void PreTreeDraw(Tree __instance, ref SpriteBatch spriteBatch, ref Vector2 tileLocation)
        {
            GameLocation location = Game1.player.currentLocation;
            int tileLocationX = (int)tileLocation.X;
            int tileLocationY = (int)tileLocation.Y;
            if (location.doesTileHaveProperty(tileLocationX, tileLocationY, "Diggable", "Back") == null 
                && !location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "Type", "Back").Equals("Grass") 
                && !location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "Type", "Back").Equals("Dirt")
                && (!MissingGrassTilesIndexes.Contains(location.getTileIndexAt(tileLocationX, tileLocationY,"Back"))
                    || !location.map.GetLayer("Back").Tiles[tileLocationX, tileLocationY].TileSheet.ImageSource.EndsWith("outdoorsTileSheet"))
                && !(location is Farm))
            {
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(new Rectangle(669, 1957, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
            }
        }

        /// <summary>
        /// Override to not let you harvest while holding the pot
        /// </summary>
        /// <param name="__result"></param>
        /// <returns></returns>
        public static bool PerformUseAction(ref bool __result)
        {
            Object activeObject = Game1.player.ActiveObject;
            if (activeObject is IndoorPot || TransplantController.IsGardenPot(activeObject))
            {
                __result = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Override to get or place a crop from/to the ground, from/into the pot.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="item"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns> 
        public static bool TryToPlaceItem(ref GameLocation location, Item item, int x, int y)
        {
            if ((Utility.withinRadiusOfPlayer(x, y, 1, Game1.player) || DataLoader.ModConfig.EnableUnlimitedRangeToTransplant) && item is Object object1 && TransplantController.IsGardenPot(object1))
            {
                Vector2 tileLocation = new Vector2((float) (x / 64), (float) (y / 64));
                if (!location.objects.ContainsKey(tileLocation))
                {
                    int tileLocationX = (int)tileLocation.X;
                    int tileLocationY = (int)tileLocation.Y;
                    if (location.isTileHoeDirt(tileLocation))
                    {
                        HoeDirt hoeDirt = location.terrainFeatures[tileLocation] as HoeDirt;
                        if (hoeDirt.crop != null)
                        {
                            if (!(Game1.player.ActiveObject is HeldIndoorPot heldPot) ||
                                !heldPot.IsHoldingSomething())
                            {
                                if (!hoeDirt.crop.forageCrop.Value || hoeDirt.crop.whichForageCrop.Value > 2)
                                {
                                    if (!hoeDirt.crop.dead.Value)
                                    {
                                        if (Game1.player.ActiveObject.Stack == 1)
                                        {
                                            CurrentHeldIndoorPot = new HeldIndoorPot(tileLocation);
                                            Game1.player.ActiveObject = null;
                                            Game1.player.ActiveObject = CurrentHeldIndoorPot;
                                            Events.GameLoop.UpdateTicked += OnUpdateTicked;
                                            HoeDirt potHoeDirt = CurrentHeldIndoorPot.hoeDirt.Value;
                                            potHoeDirt.crop = hoeDirt.crop;
                                            potHoeDirt.fertilizer.Value = hoeDirt.fertilizer.Value;
                                            TransplantController.ShakeCrop(potHoeDirt, tileLocation);
                                            hoeDirt.crop = null;
                                            hoeDirt.fertilizer.Value = 0;
                                            Game1.player.Stamina -= ((float)DataLoader.ModConfig.CropTransplantEnergyCost - (float)Game1.player.FarmingLevel * DataLoader.ModConfig.CropTransplantEnergyCost / 20f);
                                            location.playSound("dirtyHit");
                                        }
                                    }
                                }
                                return false;
                            }
                        }
                        else
                        {
                            if (Game1.player.ActiveObject is HeldIndoorPot heldPot &&
                                heldPot.hoeDirt.Value.crop != null
                                && (!heldPot.hoeDirt.Value.crop.raisedSeeds.Value || !TransplantController.IntersectWithFarmer(location, x, y))
                                )
                            {
                                if (!DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm && !Game1.player.currentLocation.IsFarm && !Game1.player.currentLocation.CanPlantSeedsHere(heldPot.hoeDirt.Value.crop.netSeedIndex.Value, tileLocationX, tileLocationY) && Game1.player.currentLocation.IsOutdoors)
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
                                    return false;
                                }
                                hoeDirt.crop = heldPot.hoeDirt.Value.crop;
                                TransplantController.ShakeCrop(hoeDirt, tileLocation);
                                hoeDirt.fertilizer.Value = heldPot.hoeDirt.Value.fertilizer.Value;
                                CleanHeldIndoorPot();
                                Game1.player.ActiveObject = (Object) RegularPotObject.getOne();
                                location.playSound("dirtyHit");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (Game1.player.ActiveObject is HeldIndoorPot heldPot 
                            && (heldPot.Tree != null || heldPot.bush.Value != null)
                            && !location.terrainFeatures.ContainsKey(tileLocation)
                            && ((heldPot.Tree != null && heldPot.Tree.isPassable()) || !TransplantController.IntersectWithFarmer(location, x, y))
                            && location.doesTileHaveProperty(tileLocationX, tileLocationY, "Water", "Back") == null
                            && !location.isTileOccupiedForPlacement(tileLocation, heldPot.Tree!= null ? new Object(heldPot.Tree.GetSeedSaplingIndex(), 1):null))
                        {
                            TerrainFeature terrainFeature;
                            if (heldPot.Tree is Tree tree)
                            {
                                if (TransplantController.IsTreeNoSpawnTile(location,tileLocationX,tileLocationY)
                                    || !location.isTileLocationOpen(new Location(x, y))
                                    || (location.doesTileHaveProperty(tileLocationX, tileLocationY, "Diggable", "Back") == null && !(location is Farm) && !location.CanPlantTreesHere(tree.GetSeedIndex(), tileLocationX, tileLocationY) && !DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021"));
                                    return false;
                                }

                                if (!TransplantController.CheckFruitTreeGrowth(location, tileLocationX, tileLocationY)) return false;

                                TransplantController.ShakeTree(tree, tileLocation);
                                terrainFeature = tree;
                            }
                            else if (heldPot.Tree is FruitTree fruitTree)
                            {
                                if (!DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree && TransplantController.IsNextToOtherTrees(location, x, y))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                                    return false;
                                }

                                if (fruitTree.growthStage.Value < FruitTree.treeStage && FruitTree.IsGrowthBlocked(tileLocation, location) && !DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth)
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", new Object(fruitTree.GetSaplingIndex(),0).DisplayName));
                                    return false;
                                }

                                if (!DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm && !(location is Farm) && !location.CanPlantTreesHere(fruitTree.GetSaplingIndex(), tileLocationX, tileLocationY))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
                                    return false;
                                }

                                if (!TransplantController.IsValidTileForFruitTree(location, fruitTree.GetSaplingIndex(), tileLocationX, tileLocationY))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068"));
                                    return false;
                                }

                                fruitTree.GreenHouseTree = !location.IsOutdoors || location.IsGreenhouse || (((int)fruitTree.treeType.Value == 7 || (int)fruitTree.treeType.Value == 8) && location is IslandWest);
                                fruitTree.GreenHouseTileTree = location.doesTileHaveProperty(tileLocationX, tileLocationY, "Diggable", "Back") == null && !location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "Type", "Back").Equals("Grass") && !location.doesTileHavePropertyNoNull(x, y, "Type", "Back").Equals("Dirt") && !(location is Farm);

                                TransplantController.ShakeTree(fruitTree, tileLocation);
                                terrainFeature = fruitTree;
                            }
                            else if (heldPot.bush.Value is Bush bush)
                            {
                                if (!TransplantController.CheckFruitTreeGrowth(location, tileLocationX, tileLocationY)) return false;

                                if (!DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm && !(location is Farm) && !location.CanPlantTreesHere(bush.GetSaplingIndex(), tileLocationX, tileLocationY))
                                {
                                    return true;
                                }

                                if (!TransplantController.IsValidTileForBush(location, tileLocationX, tileLocationY))
                                {
                                    return true;
                                }
                                DataLoader.Helper.Reflection.GetField<float>(bush, "yDrawOffset").SetValue(0f);
                                terrainFeature = TransplantController.PrepareBushForPlacement(bush, tileLocation);
                            }
                            else
                            {
                                return true;
                            }

                            location.terrainFeatures.Add(tileLocation, terrainFeature);

                            CleanHeldIndoorPot();
                            Game1.player.ActiveObject = (Object) RegularPotObject.getOne();
                            location.playSound("dirtyHit");
                            return false;
                            
                        }
                        else
                        {
                            if (location.terrainFeatures.ContainsKey(tileLocation) 
                                && (TransplantController.IsValidTree(location.terrainFeatures[tileLocation]) || TransplantController.IsValidBush(location.terrainFeatures[tileLocation]))
                                && ! (Game1.player.ActiveObject is HeldIndoorPot))
                            {
                                TerrainFeature terrainFeature = location.terrainFeatures[tileLocation];
                                if (Game1.player.ActiveObject.Stack == 1)
                                {
                                    CurrentHeldIndoorPot = new HeldIndoorPot(tileLocation);
                                    Game1.player.ActiveObject = null;
                                    Game1.player.ActiveObject = CurrentHeldIndoorPot;
                                    Events.GameLoop.UpdateTicked += OnUpdateTicked;
                                    float transplantEnergyCost = 0;
                                    GameLocation oldGameLocation = terrainFeature.currentLocation;
                                    if (terrainFeature is Tree tree)
                                    {
                                        CurrentHeldIndoorPot.Tree = tree;
                                        TransplantController.ShakeTree(tree, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[Math.Min(4, tree.growthStage.Value >=4 ? tree.growthStage.Value-1 : tree.growthStage.Value)];
                                    }
                                    else if (terrainFeature is FruitTree fruitTree)
                                    {
                                        CurrentHeldIndoorPot.Tree = fruitTree;
                                        TransplantController.ShakeTree(fruitTree, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[Math.Min(4, fruitTree.growthStage.Value)];
                                    }
                                    else if (terrainFeature is Bush bush)
                                    {
                                        CurrentHeldIndoorPot.bush.Value = bush;
                                        TransplantController.ShakeBush(bush, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.CropTransplantEnergyCost; 
                                    }
                                    location.terrainFeatures.Remove(tileLocation);
                                    terrainFeature.currentLocation = oldGameLocation;

                                    float playerSkillLevel = (float) (terrainFeature is Bush ? Game1.player.FarmingLevel : Game1.player.ForagingLevel);
                                    Game1.player.Stamina -= ((float)transplantEnergyCost - playerSkillLevel * transplantEnergyCost / 20f);
                                    location.playSound("dirtyHit");
                                }
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (TransplantController.IsTapper(location.objects[tileLocation]))
                    {
                        Game1.showRedMessage(DataLoader.I18N.Get("Tree.Grab.CantMoveWithTapper"));
                    }
                }
                
            }

            return true;
        }

        internal static void CleanHeldIndoorPot()
        {
            Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            CurrentHeldIndoorPot = null;
            Game1.player.ActiveObject = null;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.ActiveObject is HeldIndoorPot heldPot)
            {
                HoeDirt potHoeDirt = heldPot.hoeDirt.Value;
                if (potHoeDirt.crop != null)
                {
                    potHoeDirt.tickUpdate(Game1.currentGameTime, heldPot.TileLocation, Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        TransplantController.ShakeCrop(potHoeDirt);
                    }
                }
                else if (heldPot.Tree is Tree tree)
                {
                    tree.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        TransplantController.ShakeTree(tree);
                    }
                }
                else if (heldPot.Tree is FruitTree fruitTree)
                {
                    fruitTree.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        TransplantController.ShakeTree(fruitTree);
                    }
                }
                else if (heldPot.bush.Value is Bush bush)
                {
                    bush.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        TransplantController.ShakeBush(bush);
                    }
                }
            }
            else
            {
                if (CurrentHeldIndoorPot != null && Game1.player.getIndexOfInventoryItem(CurrentHeldIndoorPot) != -1 && Game1.player.Items[Game1.player.getIndexOfInventoryItem(CurrentHeldIndoorPot)] is HeldIndoorPot inventoryHeldPot)
                {
                    var f = Game1.player;
                    Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    if (inventoryHeldPot.hoeDirt.Value.crop?.currentPhase.Value < 1 
                        || (inventoryHeldPot.Tree as Tree)?.growthStage.Value < 1 )
                    {
                        multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite[1]
                        {
                            new TemporaryAnimatedSprite("TileSheets\\animations",new Rectangle(0, 12 * 64, 64, 64), 100f,8,0, f.Position + new Vector2(0.0f, -120f), false, false, (float)((double)f.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, false)
                        });
                        Game1.currentLocation.playSound("dirtyHit");
                    }
                    else
                    {
                        multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite[1]
                        {
                            new TemporaryAnimatedSprite("TileSheets\\animations",new Rectangle(0, 50 * 64, 64, 64), 100f,8,0, f.Position + new Vector2(0.0f, -120f), false, false, (float)((double)f.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.ForestGreen, 1f, 0.0f, 0.0f, 0.0f, false)
                        });
                        Game1.currentLocation.playSound("grassyStep");
                    }
                    
                    int heldIndorPotInveotryPosition = Game1.player.getIndexOfInventoryItem(CurrentHeldIndoorPot);
                    Game1.player.removeItemFromInventory(CurrentHeldIndoorPot);
                    Game1.player.addItemToInventory(RegularPotObject.getOne(), heldIndorPotInveotryPosition);
                }
                
                CurrentHeldIndoorPot = null;
                Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }
    }
}