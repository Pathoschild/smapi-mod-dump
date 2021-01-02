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
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
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
                    || !Utility.tileWithinRadiusOfPlayer((int)key.X, (int)key.Y, 1, Game1.player))
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
                            ShakeCrop(holdenHoeDirt, pot.TileLocation);
                            Game1.player.Stamina -= ((float)DataLoader.ModConfig.CropTransplantEnergyCost - (float)Game1.player.FarmingLevel * DataLoader.ModConfig.CropTransplantEnergyCost / 20f);
                            Game1.player.ActiveObject = CurrentHeldIndoorPot;
                            Events.GameLoop.UpdateTicked += OnUpdateTicked;
                        }
                        else if (pot.bush.Value is Bush bush)
                        {
                            CurrentHeldIndoorPot = new HeldIndoorPot(pot.TileLocation);
                            CurrentHeldIndoorPot.bush.Value = bush;
                            Bush holdenBush = CurrentHeldIndoorPot.bush.Value;
                            ShakeBush(holdenBush);
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
        public static bool PlayerCanPlaceItemHere(ref GameLocation location, Item item, ref int x, ref int y, ref Farmer f, ref bool __result)
        {
            if (item != null && item is Object object1 && IsGardenPot(object1) && object1.Stack == 1)
            {
                if ((Game1.eventUp || f.bathingClothes.Value) || !Utility.withinRadiusOfPlayer(x, y, 1, f) && (!Utility.withinRadiusOfPlayer(x, y, 2, f) || !Game1.isAnyGamePadButtonBeingPressed() || (double)Game1.mouseCursorTransparency != 0.0))
                    return true;
                Vector2 tileLocation = new Vector2((float)(x / 64), (float)(y / 64));
                if (!(object1 is HeldIndoorPot heldPot))
                {
                    if (location.isTileHoeDirt(tileLocation) && !location.objects.ContainsKey(tileLocation))
                    {
                        HoeDirt hoeDirt = location.terrainFeatures[tileLocation] as HoeDirt;
                        if (hoeDirt.crop != null)
                        {
                            __result = true;
                            return false;
                        }
                    }
                    else if (location.terrainFeatures.ContainsKey(tileLocation) 
                             && IsValidTree(location.terrainFeatures[tileLocation])
                             && !location.objects.ContainsKey(tileLocation))
                    {
                        __result = true;
                        return false;
                    }
                }
                else
                {
                    if (heldPot.tree != null)
                    {
                        if(!location.terrainFeatures.ContainsKey(tileLocation)
                           && !location.objects.ContainsKey(tileLocation) 
                           && !location.farmers.Any(fs => fs.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64)))
                           && location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Water", "Back") == null
                           && !location.isTileOccupied(tileLocation, ""))
                        {
                            if (heldPot.tree is Tree)
                            {
                                string str = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
                                if ((str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True")))
                                    || (!location.isTileLocationOpen(new Location(x, y)))
                                    || (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") == null && !(location is Farm) && !location.IsGreenhouse && !DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType))
                                {
                                    __result = false;
                                    return false;
                                }

                                __result = true;
                                return false;
                            }
                            else if (heldPot.tree is FruitTree)
                            {
                                if ((!IsNextToOtherTrees(location,x,y) || DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree)
                                    && (location is Farm || DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm)
                                    && ((location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass")) || DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType)
                                    && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back").Equals("Tree") 
                                    || location.IsGreenhouse && (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Stone")))
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
                        if (!location.terrainFeatures.ContainsKey(tileLocation)
                            && !location.objects.ContainsKey(tileLocation)
                            && !location.farmers.Any(fs => fs.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64)))
                            && location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Water", "Back") == null
                            && !location.isTileOccupied(tileLocation, "")
                            && ((location is Farm || DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm)
                                && ((location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass")) || DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType)
                                && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back").Equals("Tree")))
                        {
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool IsNextToOtherTrees(GameLocation location, int x,  int y)
        {
            Vector2 key = new Vector2();
            for (int index2 = x / 64 - 2; index2 <= x / 64 + 2; ++index2)
            {
                for (int index3 = y / 64 - 2; index3 <= y / 64 + 2; ++index3)
                {
                    key.X = (float)index2;
                    key.Y = (float)index3;
                    if (location.terrainFeatures.ContainsKey(key) && (location.terrainFeatures[key] is Tree || location.terrainFeatures[key] is FruitTree))
                    {
                        return true;
                    }
                }
            }
            return false;
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
            if (activeObject is IndoorPot || IsGardenPot(activeObject))
            {
                __result = false;
                return false;
            } else if (who.CurrentTool == null && activeObject == null && Game1.currentLocation.Objects.ContainsKey(index) && Game1.currentLocation.Objects[index] is IndoorPot pot)
            {
                Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int)index.X, (int)index.Y, 1, who) ? 0.5f : 1f;
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
                && IsGardenPot(Game1.player.ActiveObject)
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
        public static void PreTreeDraw(ref SpriteBatch spriteBatch, ref Vector2 tileLocation)
        {
            GameLocation location = Game1.player.currentLocation;
            
            if (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") == null && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass") && !(location is Farm))
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
            if (activeObject is IndoorPot || IsGardenPot(activeObject))
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
        [HarmonyPriority(800)]
        public static bool TryToPlaceItem(ref GameLocation location, Item item, ref int x, ref int y)
        {
            if (Utility.withinRadiusOfPlayer(x, y, 1, Game1.player) && item is Object object1 && IsGardenPot(object1))
            {
                Vector2 tileLocation = new Vector2((float) (x / 64), (float) (y / 64));
                if (!location.objects.ContainsKey(tileLocation))
                {
                    if (location.isTileHoeDirt(tileLocation))
                    {
                        HoeDirt hoeDirt = location.terrainFeatures[tileLocation] as HoeDirt;
                        if (hoeDirt.crop != null)
                        {
                            if (!(Game1.player.ActiveObject is HeldIndoorPot heldPot) ||
                                !heldPot.IsHoldingSomething())
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
                                    ShakeCrop(potHoeDirt, tileLocation);
                                    hoeDirt.crop = null;
                                    hoeDirt.fertilizer.Value = 0;
                                    Game1.player.Stamina -= ((float)DataLoader.ModConfig.CropTransplantEnergyCost - (float)Game1.player.FarmingLevel * DataLoader.ModConfig.CropTransplantEnergyCost/20f);
                                    location.playSound("dirtyHit");
                                }
                                return false;
                            }
                        }
                        else
                        {
                            if (Game1.player.ActiveObject is HeldIndoorPot heldPot &&
                                heldPot.hoeDirt.Value.crop != null
                                && !location.farmers.Any(f=>f.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64))))
                            {
                                if (!DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm && !Game1.player.currentLocation.IsFarm && !Game1.player.currentLocation.CanPlantSeedsHere(heldPot.hoeDirt.Value.crop.netSeedIndex.Value, x/64, y/64) && Game1.player.currentLocation.IsOutdoors)
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
                                    return false;
                                }
                                hoeDirt.crop = heldPot.hoeDirt.Value.crop;
                                ShakeCrop(hoeDirt, tileLocation);
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
                            && (heldPot.tree != null || heldPot.bush.Value != null)
                            && !location.terrainFeatures.ContainsKey(tileLocation)
                            && !location.farmers.Any(f => f.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64)))
                            && location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Water", "Back") == null
                            && !location.isTileOccupied(tileLocation, ""))
                        {
                            TerrainFeature terrainFeature;
                            if (heldPot.tree is Tree tree)
                            {
                                string str = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
                                if ((str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True")))
                                    || (!location.isTileLocationOpen(new Location(x, y)))
                                    || (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") == null && !(location is Farm) && !location.IsGreenhouse && !DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021"));
                                    return false;
                                }

                                ShakeTree(tree, tileLocation);
                                terrainFeature = tree;
                            }
                            else if (heldPot.tree is FruitTree fruitTree)
                            {
                                if (!DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree &&
                                    IsNextToOtherTrees(location, x, y))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                                    return false;
                                }

                                if (!DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm &&!(location is Farm) && !location.IsGreenhouse)
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
                                    return false;
                                }

                                if (!(((location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass")) || DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType)
                                    && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back").Equals("Tree")
                                    || location.IsGreenhouse && (location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Stone"))))
                                {
                                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068"));
                                    return false;
                                }

                                fruitTree.GreenHouseTree = !location.IsOutdoors || location.IsGreenhouse;
                                fruitTree.GreenHouseTileTree = location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") == null && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass") && !(location is Farm);

                                ShakeTree(fruitTree, tileLocation);
                                terrainFeature = fruitTree;
                            }
                            else if (heldPot.bush.Value is Bush bush)
                            {
                                if (!DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm && !(location is Farm) && !location.IsGreenhouse)
                                {
                                    return true;
                                }

                                if (!(((location.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "Type", "Back").Equals("Grass")) || DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType)
                                      && !location.doesTileHavePropertyNoNull((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back").Equals("Tree")))
                                {
                                    return true;
                                }

                                terrainFeature = PrepareBushForPlacement(bush, tileLocation);
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
                                && (IsValidTree(location.terrainFeatures[tileLocation]) || IsValidBush(location.terrainFeatures[tileLocation]))
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
                                    if (terrainFeature is Tree tree)
                                    {
                                        CurrentHeldIndoorPot.tree = tree;
                                        ShakeTree(tree, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[Math.Min(4, tree.growthStage.Value >=4 ? tree.growthStage.Value-1 : tree.growthStage.Value)];
                                    }
                                    else if (terrainFeature is FruitTree fruitTree)
                                    {
                                        CurrentHeldIndoorPot.tree = fruitTree;
                                        ShakeTree(fruitTree, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[Math.Min(4, fruitTree.growthStage.Value)];
                                    }
                                    else if (terrainFeature is Bush bush)
                                    {
                                        CurrentHeldIndoorPot.bush.Value = bush;
                                        ShakeBush(bush, tileLocation);
                                        transplantEnergyCost = DataLoader.ModConfig.CropTransplantEnergyCost; 
                                    }
                                    location.terrainFeatures.Remove(tileLocation);

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
                    if (location.objects[tileLocation].ParentSheetIndex == 105 && location.objects[tileLocation].bigCraftable.Value)
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

        internal static Bush PrepareBushForPlacement(Bush bush, Vector2 tileLocation)
        {
            if (!Game1.player.currentLocation.IsOutdoors && !bush.greenhouseBush.Value)
            {
                bush.greenhouseBush.Value = true;
            }
            else if (Game1.player.currentLocation.IsOutdoors && bush.greenhouseBush.Value)
            {
                bush.greenhouseBush.Value = false;
            }
            ShakeBush(bush, tileLocation);
            return bush;
        }

        private static bool IsValidTree(TerrainFeature terrainFeature)
        {
            bool result = false;
            if (terrainFeature is Tree tree)
            {
                result = Math.Min(5, tree.growthStage.Value) <= DataLoader.ModConfig.TreeTransplantMaxStage - (DataLoader.ModConfig.TreeTransplantMaxStage < 4 ? 1 : 0);
            }
            else if (terrainFeature is FruitTree fruitTree)
            {
                result = Math.Min(5, fruitTree.growthStage.Value) <= DataLoader.ModConfig.FruitTreeTransplantMaxStage - 1;
            }
            return result;
        }

        private static bool IsValidBush(TerrainFeature terrainFeature)
        {
            bool result = false;
            if (terrainFeature is Bush bush)
            {
                result = bush.size.Value == Bush.greenTeaBush;
            }
            return result;
        }

        internal static void ShakeCrop(HoeDirt hoeDirt, Vector2? tileLocation = null)
        {
            var maxShake = hoeDirt.getMaxShake();
            if (hoeDirt.crop != null && hoeDirt.crop.currentPhase.Value != 0 && (double) maxShake == 0.0)
            {
                Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
                Grass.grassSound.Play();

                var farmer = Game1.player;
                int speedOfCollision = 2;
                
                hoeDirt.shake(
                    (float)(0.392699092626572 / (double)((5 + farmer.addedSpeed) / speedOfCollision) -(speedOfCollision > 2 ? (double)hoeDirt.crop.currentPhase.Value * 3.14159274101257 / 64.0: 0.0))
                    , (float)Math.PI / 80f / (float)((5 + farmer.addedSpeed) / speedOfCollision)
                    ,tileLocation.HasValue ? (double)farmer.lastPosition.X > (double)tileLocation.Value.X * 64.0 + 32.0 : farmer.FacingDirection == 1 ? true : farmer.FacingDirection == 3 ? false : ShakeFlag = !ShakeFlag);
            }
        }

        internal static void ShakeTree(Tree tree, Vector2? tileLocation = null)
        {
            tree.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
        }

        internal static void ShakeTree(FruitTree tree, Vector2? tileLocation = null)
        {
            tree.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
        }

        internal static void ShakeBush(Bush bush, Vector2? tileLocation = null)
        {
            bush.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
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
                        ShakeCrop(potHoeDirt);
                    }
                }
                else if (heldPot.tree is Tree tree)
                {
                    tree.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        ShakeTree(tree);
                    }
                }
                else if (heldPot.tree is FruitTree fruitTree)
                {
                    fruitTree.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        ShakeTree(fruitTree);
                    }
                }
                else if (heldPot.bush.Value is Bush bush)
                {
                    bush.tickUpdate(Game1.currentGameTime, Game1.player.getTileLocation(), Game1.currentLocation);
                    if (Game1.player.isMoving() && !Game1.eventUp)
                    {
                        ShakeBush(bush);
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
                        || (inventoryHeldPot.tree as Tree)?.growthStage.Value < 1 )
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

        private static bool IsGardenPot(Object o)
        {
            return o is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value;
        }
    }
}