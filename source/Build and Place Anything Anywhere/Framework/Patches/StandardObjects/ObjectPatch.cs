/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.External.CustomBush;
using Common.Helpers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal sealed class ObjectPatch : PatchHelper
    {
        internal ObjectPatch() : base(typeof(SObject)) { }
        internal void Apply()
        {
            Patch(PatchType.Prefix, nameof(SObject.placementAction), nameof(PlacementActionPrefix), [typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer)]);
            Patch(PatchType.Prefix, nameof(SObject.canBePlacedHere), nameof(CanBePlacedHerePrefix), [typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool)]);
            Patch(PatchType.Prefix, "canPlaceWildTreeSeed", nameof(CanPlaceWildTreeSeedPrefix));
        }

        // Object placement action
        private static bool PlacementActionPrefix(SObject __instance, GameLocation location, int x, int y, ref bool __result, Farmer? who = null)
        {
            if (!ModEntry.Config.EnablePlacing)
                return true;

            Vector2 placementTile = new(x / 64, y / 64);
            __instance.setHealth(10);
            __instance.Location = location;
            __instance.TileLocation = placementTile;
            __instance.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

            // Do not allow placing objects on top of each-other.
            if (location.objects.ContainsKey(placementTile))
            {
                __result = false;
                return false;
            }
            if (!__instance.bigCraftable.Value && __instance is not Furniture)
            {
                if (__instance.IsSprinkler() && location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "NoSprinklers", "Back") == "T")
                {
                    return true;
                }
                if (__instance.IsWildTreeSapling())
                {
                    return true;
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
                SObject toPlace = (SObject)__instance.getOne();
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
                            Chest fridge = new("216", placementTile, 217, 2)
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

                            foreach (KeyValuePair<Vector2, SObject> o2 in location.objects.Pairs)
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
                            if (!obelisk1.Equals(Vector2.Zero) && !obelisk2.Equals(Vector2.Zero) && !ModEntry.Config.MultipleMiniObelisks)
                            {
                                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
                                __result = false;
                                return false;
                            }

                            toPlace.shakeTimer = 50;
                            toPlace.TileLocation = placementTile;
                            toPlace.performDropDownAction(who);

                            location.objects.Add(placementTile, toPlace);
                            toPlace.initializeLightSource(placementTile);
                            location.playSound("woodyStep");
                            return false;
                        }
                    case "(BC)254": // Ostrich Incubator
                        toPlace.shakeTimer = 50;
                        toPlace.TileLocation = placementTile;
                        toPlace.performDropDownAction(who);

                        location.objects.Add(placementTile, toPlace);
                        toPlace.initializeLightSource(placementTile);
                        location.playSound("woodyStep");
                        return false;
                }
            }
            if (__instance.Category == -19 && location.terrainFeatures.TryGetValue(placementTile, out var terrainFeature3) && terrainFeature3 is HoeDirt { crop: not null } dirt3 && __instance.QualifiedItemId is "(O)369" or "(O)368" && dirt3.crop.currentPhase.Value != 0)
            {
                return true;
            }
            // Bypass fruit tree placement checks
            if (__instance.isSapling())
            {
                if ((__instance.IsWildTreeSapling() && !ModEntry.Config.EnablePlacing && !ModEntry.Config.EnablePlanting) ||
                    (__instance.IsFruitTreeSapling() && !ModEntry.Config.EnablePlacing && !ModEntry.Config.EnablePlanting))
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
                    if (terrainFeature2 is not HoeDirt { crop: null })
                    {
                        __result = false;
                        return false;
                    }
                    location.terrainFeatures.Remove(placementTile);
                }
                string? deniedMessage2 = null;
                bool canDig = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Diggable", "Back") != null;
                string tileType = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Type", "Back");
                bool canPlantTrees = location.doesEitherTileOrTileIndexPropertyEqual((int)placementTile.X, (int)placementTile.Y, "CanPlantTrees", "Back", "T");
                if ((location is Farm && (canDig || tileType == "Grass" || tileType == "Dirt" || canPlantTrees) && (!location.IsNoSpawnTile(placementTile, "Tree") || canPlantTrees)) || ((canDig || tileType == "Stone") && location.CanPlantTreesHere(__instance.ItemId, (int)placementTile.X, (int)placementTile.Y, out deniedMessage2)) || ModEntry.Config.EnablePlacing)
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
                    FruitTree fruitTree = new(__instance.ItemId)
                    {
                        GreenHouseTileTree = (location.IsGreenhouse && tileType == "Stone")
                    };
                    fruitTree.growthRate.Value = Math.Max(1, __instance.Quality + 1);
                    location.terrainFeatures.Add(placementTile, fruitTree);
                    __result = true;
                    return false;
                }
                deniedMessage2 ??= Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068");
                Game1.showRedMessage(deniedMessage2);
                __result = false;
                return false;
            }
            if (__instance.Category is -74 or -19)
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

        // object placement
        private static bool CanBePlacedHerePrefix(SObject __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
        {
            if (!ModEntry.Config.EnablePlacing) return true;
            if (!ModEntry.Config.EnableFreePlace) return true;

            __result = !l.objects.ContainsKey(tile);
            return false;
        }

        // TODO: Rewrite tree code
        private static bool CanPlaceWildTreeSeedPrefix(SObject __instance, GameLocation? location, Vector2 tile, ref bool __result, out string? deniedMessage)
        {
            deniedMessage = null;

            // Suppress __result is never used before the body warning
            if (location == null) return true;

            if (location.objects.ContainsKey(tile))
            {
                __result = false;
                return false;
            }

            if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature) && terrainFeature is not HoeDirt)
            {
                __result = false;
                return false;
            }

            if (!location.CanPlantTreesHere(__instance.ItemId, (int)tile.X, (int)tile.Y, out deniedMessage) && !ModEntry.Config.EnablePlacing)
            {
                __result = false;
                return false;
            }

            if (ModEntry.Config.EnableFreePlace)
            {
                __result = true;
                return false;
            }

            __result = location.CheckItemPlantRules(__instance.QualifiedItemId, isGardenPot: false, true, out deniedMessage);
            return false;
        }
    }
}