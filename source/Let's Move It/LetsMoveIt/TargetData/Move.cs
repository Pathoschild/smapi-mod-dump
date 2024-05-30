/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        /// <summary>Move the current target.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="tile">The current tile position.</param>
        /// <param name="overwriteTile">To Overwrite existing Object.</param>
        public static void MoveTo(GameLocation location, Vector2 tile, bool overwriteTile)
        {
            if (!Config.ModEnabled)
            {
                TargetObject = null;
                return;
            }
            if (TargetObject is null)
                return;

            if (TargetObject is Farmer farmer)
            {
                farmer.Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                TargetObject = null;
            }
            else if (TargetObject is NPC character)
            {
                if (location == TargetLocation)
                {
                    character.Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                }
                else
                {
                    Game1.warpCharacter(character, location, (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2() / 64);
                }
                if (character is not Monster)
                    character.Halt();
                TargetObject = null;
            }
            else if (TargetObject is FarmAnimal farmAnimal)
            {
                if (location != TargetLocation)
                {
                    TargetLocation.animals.Remove(farmAnimal.myID.Value);
                    location.animals.TryAdd(farmAnimal.myID.Value, farmAnimal);
                }
                farmAnimal.Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                TargetObject = null;
            }
            else if (TargetObject is SObject sObject)
            {
                if (TargetLocation.objects.ContainsKey(TilePosition))
                {
                    TargetLocation.objects.Remove(TilePosition);
                    if (location.objects.ContainsKey(tile))
                    {
                        location.objects.Remove(tile);
                    }
                    location.objects.Add(tile, sObject);
                    TargetObject = null;
                }
                else
                {
                    TargetObject = null;
                    Game1.playSound("dwop");
                    return;
                }
            }
            else if (TargetObject is ResourceClump resourceClump)
            {
                int index = TargetLocation.resourceClumps.IndexOf(resourceClump);
                if (index >= 0)
                {
                    if (location == TargetLocation)
                    {
                        location.resourceClumps[index].netTile.Value = tile;
                        TargetObject = null;
                    }
                    else
                    {
                        TargetLocation.resourceClumps.Remove(resourceClump);
                        location.resourceClumps.Add(resourceClump);
                        int newIndex = location.resourceClumps.IndexOf(resourceClump);
                        location.resourceClumps[newIndex].netTile.Value = tile;
                        TargetObject = null;
                    }
                }
                else
                {
                    TargetObject = null;
                    Game1.playSound("dwop");
                    return;
                }
            }
            else if (TargetObject is TerrainFeature terrainFeature)
            {
                if (TargetObject is Bush bush && bush.size.Value == 3)
                {
                    if (location.objects.TryGetValue(tile, out var obj))
                    {
                        if (obj is IndoorPot pot)
                        {
                            if (pot.bush.Value is not null || pot.hoeDirt.Value.crop is not null)
                            {
                                Game1.playSound("cancel");
                                return;
                            }
                        }
                    }
                    if (TargetLocation.objects.TryGetValue(TilePosition, out var obj1) && obj1 is IndoorPot pot1 && pot1.bush.Value is not null)
                    {
                        pot1.bush.Value = null;
                    }
                    else if (TargetLocation.terrainFeatures.ContainsKey(TilePosition))
                    {
                        TargetLocation.terrainFeatures.Remove(TilePosition);
                    }
                    else
                    {
                        TargetObject = null;
                        Game1.playSound("dwop");
                        return;
                    }
                    if (location.objects.TryGetValue(tile, out var obj2) && obj2 is IndoorPot pot2)
                    {
                        bush.inPot.Value = true;
                        bush.netTilePosition.Value = tile;
                        pot2.bush.Value = bush;
                        TargetObject = null;
                    }
                    else
                    {
                        if (bush.inPot.Value)
                        {
                            bush.inPot.Value = false;
                            bush.GetType().GetField("yDrawOffset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(bush, 0f);
                        }
                        if (location.terrainFeatures.ContainsKey(tile))
                        {
                            location.terrainFeatures.Remove(tile);
                        }
                        location.terrainFeatures.Add(tile, terrainFeature);
                        TargetObject = null;
                    }
                }
                else if (TargetObject is LargeTerrainFeature largeTerrainFeature && TargetLocation.largeTerrainFeatures.Contains(largeTerrainFeature))
                {
                    int index = TargetLocation.largeTerrainFeatures.IndexOf(largeTerrainFeature);
                    if (index >= 0)
                    {
                        if (location == TargetLocation)
                        {
                            location.largeTerrainFeatures[index].netTilePosition.Value = tile;
                            TargetObject = null;
                        }
                        else
                        {
                            TargetLocation.largeTerrainFeatures.Remove(largeTerrainFeature);
                            location.largeTerrainFeatures.Add(largeTerrainFeature);
                            int newIndex = location.largeTerrainFeatures.IndexOf(largeTerrainFeature);
                            location.largeTerrainFeatures[newIndex].netTilePosition.Value = tile;
                            TargetObject = null;
                        }
                    }
                    else
                    {
                        TargetObject = null;
                        Game1.playSound("dwop");
                        return;
                    }
                }
                else if (TargetLocation.terrainFeatures.ContainsKey(TilePosition))
                {
                    TargetLocation.terrainFeatures.Remove(TilePosition);
                    if (location.terrainFeatures.ContainsKey(tile))
                    {
                        location.terrainFeatures.Remove(tile);
                    }
                    location.terrainFeatures.Add(tile, terrainFeature);
                    HashSet<Vector2> neighbors = [tile + new Vector2(0, 1), tile + new Vector2(1, 0), tile + new Vector2(0, -1), tile + new Vector2(-1, 0)];
                    foreach (Vector2 ct in neighbors)
                    {
                        if (location.terrainFeatures.ContainsKey(ct))
                        {
                            if (location.terrainFeatures[ct] is HoeDirt hoeDirtNeighbors)
                            {
                                hoeDirtNeighbors.updateNeighbors();
                            }
                        }
                    }
                    if (terrainFeature is HoeDirt hoeDirt)
                    {
                        hoeDirt.updateNeighbors();
                        hoeDirt.crop?.updateDrawMath(tile);
                    }
                    TargetObject = null;
                }
            }
            else if (TargetObject is Crop crop)
            {
                if (location.isCropAtTile((int)tile.X, (int)tile.Y) || !location.isTileHoeDirt(tile))
                {
                    Game1.playSound("cancel");
                    return;
                }
                if (location.objects.TryGetValue(tile, out var isPot))
                {
                    if (isPot is IndoorPot pot)
                    {
                        if (pot.bush.Value is not null || pot.hoeDirt.Value.crop is not null)
                        {
                            Game1.playSound("cancel");
                            return;
                        }
                    }
                }
                if (TargetLocation.objects.TryGetValue(TilePosition, out var oldPot))
                {
                    if (oldPot is IndoorPot pot && pot.hoeDirt.Value.crop is not null)
                    {
                        pot.hoeDirt.Value.crop = null;
                    }
                    else
                    {
                        TargetObject = null;
                        Game1.playSound("dwop");
                        return;
                    }
                }
                else if (TargetLocation.terrainFeatures.TryGetValue(TilePosition, out var oldHoeDirt))
                {
                    if (oldHoeDirt is HoeDirt hoeDirt && hoeDirt.crop is not null)
                    {
                        hoeDirt.crop = null;
                    }
                    else
                    {
                        TargetObject = null;
                        Game1.playSound("dwop");
                        return;
                    }
                }
                else
                {
                    TargetObject = null;
                    Game1.playSound("dwop");
                    return;
                }
                if (location.objects.TryGetValue(tile, out var newPot))
                {
                    if (newPot is IndoorPot pot)
                    {
                        pot.hoeDirt.Value.crop = crop;
                        pot.hoeDirt.Value.crop.updateDrawMath(tile);
                        TargetObject = null;
                    }
                }
                else if (location.terrainFeatures.TryGetValue(tile, out var newHoeDirt))
                {
                    if (newHoeDirt is HoeDirt hoeDirt)
                    {
                        hoeDirt.crop = crop;
                        hoeDirt.crop.updateDrawMath(tile);
                        TargetObject = null;
                    }
                }
            }
            else if (TargetObject is Building building)
            {
                if (location.IsBuildableLocation())
                {
                    if (location.buildStructure(building, tile - TileOffset, Game1.player, overwriteTile))
                    {
                        if (TargetObject is ShippingBin shippingBin)
                        {
                            shippingBin.initLid();
                        }
                        if (TargetObject is GreenhouseBuilding)
                        {
                            Game1.getFarm().greenhouseMoved.Value = true;
                        }
                        building.performActionOnBuildingPlacement();
                        TargetObject = null;
                    }
                    else
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                }
            }
            if (TargetObject is null)
            {
                PlaySound();
            }
            else
            {
                TargetObject = null;
                Game1.playSound("dwop");
            }
        }
    }
}
