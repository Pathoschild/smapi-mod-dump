/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Additions.Clumps;
using ItemExtensions.Models;
using ItemExtensions.Models.Enums;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ItemExtensions;

public interface IApi
{
    /// <summary>
    /// Checks for resource data with the Stone type.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool IsStone(string id);
    
    /// <summary>
    /// Checks for resource data in the mod.
    /// </summary>
    /// <param name="id">Qualified item ID</param>
    /// <param name="health">MinutesUntilReady value</param>
    /// <param name="itemDropped">Item dropped by ore</param>
    /// <returns>Whether the object has ore data.</returns>
    bool IsResource(string id, out int? health, out string itemDropped);
    
    /// <summary>
    /// Checks mod's menu behaviors. If a target isn't provided, it'll search whether any exist.
    /// </summary>
    /// <param name="qualifiedItemId">Qualified item ID.</param>
    /// <param name="target">Item to search behavior for. (Qualified item ID)</param>
    /// <returns>Whether this item has menu behavior for target.</returns>
    bool HasBehavior(string qualifiedItemId, string target);

    bool IsClump(string qualifiedItemId);
    
    bool TrySpawnClump(string itemId, Vector2 position, string locationName, out string error, bool avoidOverlap = false);
    
    bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = false);

    List<string> GetCustomSeeds(string itemId, bool includeSource, bool parseConditions = true);
}

//remove all of this ↓ when copying to your mod
public class Api : IApi
{
    public bool IsStone(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;
        
        if (!ModEntry.Ores.TryGetValue(id, out var resource))
            return false;

        if (resource is null || resource == new ResourceData())
            return false;

        return resource.Type == CustomResourceType.Stone;
    }

    public bool IsResource(string id, out int? health, out string itemDropped)
    {
        health = null;
        itemDropped = null;
        
        if (string.IsNullOrWhiteSpace(id))
            return false;

        if (!ModEntry.Ores.TryGetValue(id, out var resource))
            return false;

        if (resource is null || resource == new ResourceData())
            return false;
        
        health = resource.Health;
        itemDropped = resource.ItemDropped;
        return true;
    }

    public bool HasBehavior(string qualifiedItemId, string target = null)
    {
        if(string.IsNullOrWhiteSpace(target))
            return ModEntry.MenuActions.ContainsKey(qualifiedItemId);
        
        if (!ModEntry.MenuActions.TryGetValue(qualifiedItemId, out var value))
            return false;

        var behavior = value.Find(b => b.TargetId == target);
        return behavior != null;
    }

    public bool IsClump(string qualifiedItemId) => ModEntry.BigClumps.ContainsKey(qualifiedItemId);

    public bool TrySpawnClump(string itemId, Vector2 position, string locationName, out string error, bool avoidOverlap = false) => TrySpawnClump(itemId, position, Utility.fuzzyLocationSearch(locationName), out error, avoidOverlap);
    
    public bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = true)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(itemId))
        {
            error = "Id can't be null.";
            return false;
        }

        if(ModEntry.BigClumps.TryGetValue(itemId, out var data) == false)
        {
            error = "Couldn't find the given ID.";
            return false;
        }

        var clump = ExtensionClump.Create(itemId, data, position);

        try
        {
            if(avoidOverlap)
            {
                if (location.IsTileOccupiedBy(position))
                {
                    var width = location.map.DisplayWidth / 64;
                    var height = location.map.DisplayHeight / 64;
                    
                    for (var i = 0; i < 30; i++)
                    {
                        var newPosition = new Vector2(
                            Game1.random.Next(1, width),
                            Game1.random.Next(1, height));
                        
                        if (location.IsTileOccupiedBy(newPosition) || location.IsNoSpawnTile(newPosition) || !location.CanItemBePlacedHere(newPosition))
                            continue;

                        clump.Tile = newPosition;
                    }
                }
            }
            
            location.resourceClumps.Add(clump);
        }
        catch (Exception ex)
        {
            error = $"{ex}";
            return false;
        }

        return true;
    }

    public List<string> GetCustomSeeds(string itemId, bool includeSource, bool parseConditions = true)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }
        
        //if no seed data
        if (ModEntry.Seeds.TryGetValue(itemId, out var seeds) == false)
            return null;

        var result = new List<string>();

        foreach (var mixedSeed in seeds)
        {
            if (string.IsNullOrWhiteSpace(mixedSeed.Condition)) 
                continue; 
            
            if (GameStateQuery.CheckConditions(mixedSeed.Condition, Game1.player.currentLocation, Game1.player))
                result.Add(mixedSeed.ItemId);
        }

        return result;
    }
}