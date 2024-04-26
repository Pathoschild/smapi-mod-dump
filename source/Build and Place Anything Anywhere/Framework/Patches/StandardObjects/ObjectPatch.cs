/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.GameData.Fences;
using StardewValley.GameData.WildTrees;
using StardewValley.GameData;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Extensions;
using Object = StardewValley.Object;
using AnythingAnywhere.Framework.External.CustomBush;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal class ObjectPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Object);

        internal ObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Object.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Object.canBePlacedHere), new[] { typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(CanBePlacedHerePrefix)));

        }

        // Object placment action
        private static bool PlacementActionPrefix(Object __instance, GameLocation location, int x, int y, ref bool __result, Farmer who = null)
        {
            if (!ModEntry.modConfig.EnablePlacing)
                return true;

            Vector2 placementTile = new Vector2(x / 64, y / 64);
            __instance.setHealth(10);
            __instance.Location = location;
            __instance.TileLocation = placementTile;
            __instance.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

            // Do not allow placing objects on top of eachother.
            if (location.objects.ContainsKey(placementTile))
            {
                __result = false;
                return false;
            }
            if (!__instance.bigCraftable.Value && !(__instance is Furniture))
            {
                if (__instance.IsSprinkler() && location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "NoSprinklers", "Back") == "T")
                {
                    return true;
                }
                if (__instance.IsWildTreeSapling())
                {
                    if (!canPlaceWildTreeSeed(__instance, location, placementTile, out var deniedMessage))
                    {
                        if (deniedMessage == null)
                        {
                            deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021");
                        }
                        Game1.showRedMessage(deniedMessage);
                        __result = false;
                        return false;
                    }
                    string treeType = Tree.ResolveTreeTypeFromSeed(__instance.QualifiedItemId);
                    if (treeType != null)
                    {
                        Game1.stats.Increment("wildtreesplanted");
                        location.terrainFeatures.Remove(placementTile);
                        location.terrainFeatures.Add(placementTile, new Tree(treeType, 0));
                        location.playSound("dirtyHit");
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                if (__instance.IsFloorPathItem())
                {
                    return true;
                }
                if (ItemContextTagManager.HasBaseTag(__instance.QualifiedItemId, "torch_item"))
                {
                    return true;
                }
                if (__instance.IsFenceItem())
                {
                    return true;
                }
                switch (__instance.QualifiedItemId)
                {
                    case "(O)TentKit":
                    case "(O)926":
                    case "(O)286":
                    case "(O)287":
                    case "(O)288":
                    case "(O)893":
                    case "(O)894":
                    case "(O)895":
                    case "(O)297":
                    case "(O)BlueGrassStarter":
                    case "(O)710":
                    case "(O)805":
                        return true;
                }

            }
            else
            {
                if (__instance.IsTapper())
                {
                    return true;
                }
                if (__instance.HasContextTag("sign_item"))
                {
                    return true;
                }
                if (__instance.HasContextTag("torch_item"))
                {
                    return true;
                }
                switch (__instance.QualifiedItemId)
                {
                    case "(BC)108":
                    case "(BC)109":
                    case "(BC)71":
                    case "(BC)232":
                    case "(BC)130":
                    case "(BC)BigChest":
                    case "(BC)BigStoneChest":
                    case "(BC)163":
                    case "(BC)165":
                    case "(BC)208":
                    case "(BC)209":
                    case "(BC)211":
                    case "(BC)214":
                    case "(BC)248":
                    case "(BC)256":
                    case "(BC)275":
                        return true;
                    case "(BC)216": // Mini-Fridge
                        {
                            Chest fridge = new Chest("216", placementTile, 217, 2)
                            {
                                shakeTimer = 50
                            };
                            fridge.fridge.Value = true;
                            location.objects.Add(placementTile, fridge);
                            location.playSound("hammer");
                            __result = true;
                            return false;
                        }
                    case "(BC)238": // Mini-Obelisk
                        {
                            Vector2 obelisk1 = Vector2.Zero;
                            Vector2 obelisk2 = Vector2.Zero;

                            foreach (KeyValuePair<Vector2, Object> o2 in location.objects.Pairs)
                            {
                                if (o2.Value.QualifiedItemId == "(BC)238")
                                {
                                    if (obelisk1.Equals(Vector2.Zero))
                                    {
                                        obelisk1 = o2.Key;
                                    }
                                    else if (obelisk2.Equals(Vector2.Zero))
                                    {
                                        obelisk2 = o2.Key;
                                        break;
                                    }
                                }
                            }
                            if (!obelisk1.Equals(Vector2.Zero) && !obelisk2.Equals(Vector2.Zero) && !ModEntry.modConfig.MultipleMiniObelisks)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
                                __result = false;
                                return false; //skip original method
                            }
                            break;
                        }
                    case "(BC)254": // Ostrich Incubator
                        break;
                }
            }
            if (__instance.Category == -19 && location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature3) && terrainFeature3 is HoeDirt { crop: not null } dirt3 && (__instance.QualifiedItemId == "(O)369" || __instance.QualifiedItemId == "(O)368") && (int)dirt3.crop.currentPhase.Value != 0)
            {
                return true;
            }
            // Bypass fruit tree placement checks
            if (__instance.isSapling())
            {
                if ((__instance.IsWildTreeSapling() && !ModEntry.modConfig.EnableWildTreeTweaks) || (__instance.IsFruitTreeSapling() && !ModEntry.modConfig.EnablePlacing))
                {
                    if (FruitTree.IsTooCloseToAnotherTree(new Vector2(x / 64, y / 64), location))
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                        __result = false;
                        return false;
                    }
                    if (FruitTree.IsGrowthBlocked(new Vector2(x / 64, y / 64), location))
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", __instance.DisplayName));
                        __result = false;
                        return false;
                    }
                }
                if (location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature2))
                {
                    if (!(terrainFeature2 is HoeDirt { crop: null }))
                    {
                        __result = false;
                        return false;
                    }
                    location.terrainFeatures.Remove(placementTile);
                }
                string deniedMessage2 = null;
                bool canDig = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Diggable", "Back") != null;
                string tileType = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Type", "Back");
                bool canPlantTrees = location.doesEitherTileOrTileIndexPropertyEqual((int)placementTile.X, (int)placementTile.Y, "CanPlantTrees", "Back", "T");
                if (((location is Farm && (canDig || tileType == "Grass" || tileType == "Dirt" || canPlantTrees) && (!location.IsNoSpawnTile(placementTile, "Tree") || canPlantTrees)) || ((canDig || tileType == "Stone") && location.CanPlantTreesHere(__instance.ItemId, (int)placementTile.X, (int)placementTile.Y, out deniedMessage2))) || ModEntry.modConfig.EnablePlacing)
                {
                    location.playSound("dirtyHit");
                    DelayedAction.playSoundAfterDelay("coin", 100);
                    if (__instance.IsTeaSapling())
                    {
                        var newBush = new Bush(placementTile, 3, location);
                        location.terrainFeatures.Add(placementTile, CustomBushModData.AddBushModData(newBush, __instance));
                        __result = true;
                        return false;
                    }
                    FruitTree fruitTree = new FruitTree(__instance.ItemId)
                    {
                        GreenHouseTileTree = (location.IsGreenhouse && tileType == "Stone")
                    };
                    fruitTree.growthRate.Value = Math.Max(1, __instance.Quality + 1);
                    location.terrainFeatures.Add(placementTile, fruitTree);
                    __result = true;
                    return false;
                }
                if (deniedMessage2 == null)
                {
                    deniedMessage2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068");
                }
                Game1.showRedMessage(deniedMessage2);
                __result = false;
                return false;
            }
            if (__instance.Category == -74 || __instance.Category == -19)
            {
                return true;
            }
            if (!__instance.performDropDownAction(who))
            {
                return true;
            }
            location.playSound("woodyStep");
            __result = true;
            return false;
        }

        // object placement valid
        private static bool CanBePlacedHerePrefix(Object __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
        {
            if (!ModEntry.modConfig.EnablePlacing)
                return true;

            if (ModEntry.modConfig.EnableFreePlace)
            {
                __result = true;
                return false;
            }

            if (__instance.QualifiedItemId == "(O)710")
            {
                return true;
            }
            if (__instance.IsTapper() && l.terrainFeatures.TryGetValue(tile, out var terrainFeature) && terrainFeature is Tree tree && !l.objects.ContainsKey(tile) && (tree.GetData()?.CanBeTapped() ?? false))
            {
                return true;
            }
            if (__instance.QualifiedItemId == "(O)805" && l.terrainFeatures.TryGetValue(tile, out var terrainFeature2) && terrainFeature2 is Tree)
            {
                return true;
            }
            if (Object.isWildTreeSeed(__instance.ItemId))
            {
                if (!l.CanItemBePlacedHere(tile, itemIsPassable: true, collisionMask))
                {
                    __result = false;
                    return false;
                }
                if (!canPlaceWildTreeSeed(__instance, l, tile, out var deniedMessage))
                {
                    if (showError && deniedMessage != null)
                    {
                        Game1.showRedMessage(deniedMessage);
                    }
                    __result = false;
                    return false;
                }
                __result = true;
                return false;
            }
            if ((int)__instance.Category == -74)
            {
                HoeDirt dirt = l.GetHoeDirtAtTile(tile);
                Object obj = l.getObjectAtTile((int)tile.X, (int)tile.Y);
                IndoorPot pot = obj as IndoorPot;
                if (dirt?.crop != null || (dirt == null && l.terrainFeatures.TryGetValue(tile, out var _)))
                {
                    __result = false;
                    return false;
                }
                if (__instance.IsFruitTreeSapling())
                {
                    if (obj != null)
                    {
                        __result = false;
                        return false;
                    }
                    if (dirt == null)
                    {
                        if ((FruitTree.IsTooCloseToAnotherTree(tile, l, !__instance.IsFruitTreeSapling())) && !ModEntry.modConfig.EnableFruitTreeTweaks)
                        {
                            if (showError)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                            }
                            __result = false;
                            return false;
                        }
                        if (FruitTree.IsGrowthBlocked(tile, l))
                        {
                            if (showError)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", __instance.DisplayName));
                            }
                            __result = false;
                            return false;
                        }
                        if (!l.CanItemBePlacedHere(tile, itemIsPassable: true, collisionMask))
                        {
                            __result = false;
                            return false;
                        }
                        if (!l.CanPlantTreesHere(__instance.ItemId, (int)tile.X, (int)tile.Y, out var deniedMessage2) && !ModEntry.modConfig.EnablePlacing)
                        {
                            if (showError && deniedMessage2 != null)
                            {
                                Game1.showRedMessage(deniedMessage2);
                            }
                            __result = false;
                            return false;
                        }
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
                if (__instance.IsTeaSapling())
                {
                    bool isFreeGardenPot = pot != null && pot.bush.Value == null && pot.hoeDirt.Value.crop == null;
                    if (isFreeGardenPot)
                    {
                        if (!l.IsOutdoors)
                        {
                            __result = true;
                            return false;
                        }
                    }
                    else
                    {
                        if (obj != null || dirt != null)
                        {
                            __result = false;
                            return false;
                        }
                        if (!l.CanItemBePlacedHere(tile, itemIsPassable: true, collisionMask))
                        {
                            __result = false;
                            return false;
                        }
                        if (l.IsGreenhouse && l.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") == null)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    __result = true;
                    return false;
                }
                if (__instance.IsWildTreeSapling() )
                {
                    return true;
                }
                if (__instance.HasTypeObject())
                {
                    return true;
                }
                return true;
            }
            if ((int)__instance.Category == -19)
            {
                return true;
            }
            if (l != null)
            {
                return true;
            }
            if (__instance.IsFloorPathItem())
            {
                return true;
            }
            return true;
        }

        // Reimpmenting canPlaceWildTreeSeed, as its private and can't reference.
        internal static bool canPlaceWildTreeSeed(Object __instance, GameLocation location, Vector2 tile, out string deniedMessage)
        {
            if (location.IsNoSpawnTile(tile, "Tree", ignoreTileSheetProperties: true) && !ModEntry.modConfig.EnableFreePlace)
            {
                deniedMessage = null;
                return false;
            }
            if (location.IsNoSpawnTile(tile, "Tree") && !location.doesEitherTileOrTileIndexPropertyEqual((int)tile.X, (int)tile.Y, "CanPlantTrees", "Back", "T") && !ModEntry.modConfig.EnableFreePlace)
            {
                deniedMessage = null;
                return false;
            }
            if (location.objects.ContainsKey(tile))
            {
                deniedMessage = null;
                return false;
            }
            if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature) && !(terrainFeature is HoeDirt))
            {
                deniedMessage = null;
                return false;
            }
            if (!location.CanPlantTreesHere(__instance.ItemId, (int)tile.X, (int)tile.Y, out deniedMessage) && !ModEntry.modConfig.EnablePlacing)
            {
                return false;
            }
            if (ModEntry.modConfig.EnableFreePlace)
            {
                deniedMessage = null;
                return true;
            }

            return location.CheckItemPlantRules(__instance.QualifiedItemId, isGardenPot: false, true, out deniedMessage);
        }

    }
}
