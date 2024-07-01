/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using AnythingAnywhere.Framework.External.CustomBush;

namespace AnythingAnywhere.Framework.Patches;
internal sealed class PlacementPatches : PatchHelper
{
    public void Apply()
    {
        Patch<SObject>(PatchType.Prefix, nameof(SObject.placementAction), nameof(PlacementActionPrefix), [typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer)]);
        Patch<SObject>(PatchType.Prefix, nameof(SObject.canBePlacedHere), nameof(CanBePlacedHerePrefix), [typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool)]);

        Patch<GameLocation>(PatchType.Postfix, nameof(GameLocation.CanPlaceThisFurnitureHere), nameof(CanPlaceThisFurnitureHerePostfix), [typeof(Furniture)]);
        Patch<GameLocation>(PatchType.Postfix, nameof(GameLocation.CanFreePlaceFurniture), nameof(CanFreePlaceFurniturePostfix));

        Patch<Furniture>(PatchType.Postfix, nameof(Furniture.GetAdditionalFurniturePlacementStatus), nameof(GetAdditionalFurniturePlacementStatusPostfix), [typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer)]);
        Patch<Furniture>(PatchType.Postfix, nameof(Furniture.canBePlacedHere), nameof(CanBePlacedHerePostfix), [typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool)]);
        Patch<Furniture>(PatchType.Postfix, nameof(Furniture.canBeRemoved), nameof(CanBeRemovedPostfix), [typeof(Farmer)]);
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
                        if (o2.Value.QualifiedItemId != "(BC)238") continue;
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
	    if (__instance.isSapling())
	    {
            if (!__instance.IsTeaSapling())
                return true;

            string? deniedMessage2 = null;
            bool canDig = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Diggable", "Back") != null;
            string tileType = location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Type", "Back");
            bool canPlantTrees = location.doesEitherTileOrTileIndexPropertyEqual((int)placementTile.X, (int)placementTile.Y, "CanPlantTrees", "Back", "T");
            if (((canDig || tileType == "Grass" || tileType == "Dirt" || canPlantTrees) && (!location.IsNoSpawnTile(placementTile, "Tree") || canPlantTrees)) ||
                ((canDig || tileType == "Stone") && location.CanPlantTreesHere(__instance.ItemId, (int)placementTile.X, (int)placementTile.Y, out deniedMessage2)) ||
                ModEntry.Config.EnablePlaceAnywhere)
            {
                location.playSound("dirtyHit");
                DelayedAction.playSoundAfterDelay("coin", 100);
                var newBush = new Bush(placementTile, 3, location);
                location.terrainFeatures.Add(placementTile, CustomBushModData.AddBushModData(newBush, __instance));
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
        if (!ModEntry.Config.EnablePlaceAnywhere) return true;

        __result = !l.objects.ContainsKey(tile);
        return false;
    }

    // Sets all furniture types as placeable in all locations.
    private static void CanPlaceThisFurnitureHerePostfix(GameLocation __instance, Furniture furniture, ref bool __result)
    {
        if (ModEntry.Config.EnablePlacing)
            __result = true;
    }

    // Allows longer reach when placing furniture
    private static void CanFreePlaceFurniturePostfix(GameLocation __instance, ref bool __result)
    {
        if (ModEntry.Config.EnablePlacing)
            __result = true;
    }

    // Enables disabling wall furniture in all places in decoratable locations. It can be annoying indoors.
    private static void GetAdditionalFurniturePlacementStatusPostfix(Furniture __instance, GameLocation location, int x, int y, Farmer who, ref int __result)
    {
        if (ModEntry.Config.EnablePlacing)
            __result = 0;
    }

    //Enable placing furniture in walls
    private static void CanBePlacedHerePostfix(Furniture __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
    {
        if (ModEntry.Config.EnablePlaceAnywhere)
            __result = true;
    }

    private static void CanBeRemovedPostfix(Furniture __instance, Farmer who, ref bool __result)
    {
        if (!ModEntry.Config.EnableRugRemovalBypass)
            return;

        GameLocation location = __instance.Location;
        if (location == null)
            return;

        if (__instance.isPassable())
            __result = true;
    }
}