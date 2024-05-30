/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Additions;
using ItemExtensions.Additions.Clumps;
using ItemExtensions.Models;
using ItemExtensions.Models.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Internal;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

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

    /// <summary>
    /// Checks for a qualified id in modded clump data.
    /// </summary>
    /// <param name="qualifiedItemId">Qualified item ID.</param>
    /// <returns>Whether this id is a clump's.</returns>
    bool IsClump(string qualifiedItemId);
    
    /// <summary>
    /// Tries to spawn a clump.
    /// </summary>
    /// <param name="itemId">The clump ID.</param>
    /// <param name="position">Tile position.</param>
    /// <param name="locationName">Location name or unique name.</param>
    /// <param name="error">Error string, if applicable.</param>
    /// <param name="avoidOverlap">Avoid overlapping with other clumps.</param>
    /// <returns>Whether spawning succeeded.</returns>
    bool TrySpawnClump(string itemId, Vector2 position, string locationName, out string error, bool avoidOverlap = false);
    
    /// <summary>
    /// Tries to spawn a clump.
    /// </summary>
    /// <param name="itemId">The clump ID.</param>
    /// <param name="position">Tile position.</param>
    /// <param name="location">Location to use.</param>
    /// <param name="error">Error string, if applicable.</param>
    /// <param name="avoidOverlap">Avoid overlapping with other clumps.</param>
    /// <returns>Whether spawning succeeded.</returns>
    bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = false);

    /// <summary>
    /// Checks custom mixed seeds.
    /// </summary>
    /// <param name="itemId">The 'main seed' ID.</param>
    /// <param name="includeSource">Include the main seed's crop in calculation.</param>
    /// <param name="parseConditions">Whether to pase GSQs before adding to list.</param>
    /// <returns>All possible seeds.</returns>
    List<string> GetCustomSeeds(string itemId, bool includeSource, bool parseConditions = true);

    /// <summary>
    /// Gets drops for a clump.
    /// </summary>
    /// <param name="clump">The clump instance.</param>
    /// <param name="parseConditions">Whether to pase GSQs before adding to list.</param>
    /// <returns>All possible drops, with %.</returns>
    Dictionary<string,(double, int)> GetClumpDrops(ResourceClump clump, bool parseConditions = false);

    /// <summary>
    /// Gets drops for a node.
    /// </summary>
    /// <param name="clump">The node instance.</param>
    /// <param name="parseConditions">Whether to pase GSQs before adding to list.</param>
    /// <returns>All possible drops, with %.</returns>
    Dictionary<string,(double,int)> GetObjectDrops(Object node, bool parseConditions = false);

    bool GetResourceData(string id, bool isClump, out object data);
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
        if (Game1.content.DoesAssetExist<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/{qualifiedItemId}") == false)
            return false;

        //try loading & checking behavior
        var particularMenuData = ModEntry.Help.GameContent.Load<Dictionary<string, MenuBehavior>>($"Mods/{ModEntry.Id}/MenuActions/{qualifiedItemId}");

        if (particularMenuData is null)
        {
            return false;
        }

        //if no specific target, check the asset has any data
        if (string.IsNullOrWhiteSpace(target))
            return particularMenuData.Any();
        else
        {
            //else check specific target
            return particularMenuData.ContainsKey(target);
        }
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

    public Dictionary<string,(double, int)> GetClumpDrops(ResourceClump clump, bool parseConditions = false)
    {
        var result = new Dictionary<string,(double, int)>();
        var location = Game1.player.currentLocation;
        var who = Game1.player;
        var context = new ItemQueryContext(location, who, Game1.random);

        if (clump is null)
            return result;
        
        if (clump.modData.TryGetValue(ModKeys.ClumpId, out var id) == false)
            return result;
    
        if (!ModEntry.BigClumps.TryGetValue(id, out var resource))
            return result;

        if (resource is null || resource == new ResourceData())
            return result;
            
        if(string.IsNullOrWhiteSpace(resource.ItemDropped) == false)
        {
            result.Add(resource.ItemDropped, (1, Game1.random.Next(resource.MinDrops, resource.MaxDrops)));
        }

        foreach(var drop in resource.ExtraItems)
        {
            if(parseConditions && string.IsNullOrWhiteSpace(drop.Condition) == false && GameStateQuery.CheckConditions(drop.Condition, location, who) == false)
                continue;
                
            var itemQuery = ItemQueryResolver.TryResolve(drop, context, drop.Filter, drop.AvoidRepeat);
            foreach (var queryResult in itemQuery)
            {
                var parsedItem = ItemRegistry.Create(queryResult.Item.QualifiedItemId, queryResult.Item.Stack, queryResult.Item.Quality);
                //parsedItem.Stack *= multiplier;
        
                result.Add(parsedItem.QualifiedItemId, (drop.Chance, Game1.random.Next(drop.MinStack, drop.MaxStack)));
            }
        }

        return result;
    }

    public Dictionary<string,(double, int)> GetObjectDrops(Object node, bool parseConditions = false)
    {
        var result = new Dictionary<string,(double, int)>();
        var location = Game1.player.currentLocation;
        var who = Game1.player;
        var context = new ItemQueryContext(location, who, Game1.random);

        if (node is null)
            return result;
        
        if (!ModEntry.Ores.TryGetValue(node.QualifiedItemId, out var resource))
            return result;

        if (resource is null || resource == new ResourceData())
            return result;

        if (string.IsNullOrWhiteSpace(resource.ItemDropped) == false)
        {
            result.Add(resource.ItemDropped, (1, Game1.random.Next(resource.MinDrops, resource.MaxDrops)));
        }

        foreach (var drop in resource.ExtraItems)
        {
            if (parseConditions && string.IsNullOrWhiteSpace(drop.Condition) == false && GameStateQuery.CheckConditions(drop.Condition, location, who) == false)
                continue;

            var itemQuery = ItemQueryResolver.TryResolve(drop, context, drop.Filter, drop.AvoidRepeat);
            foreach (var queryResult in itemQuery)
            {
                var parsedItem = ItemRegistry.Create(queryResult.Item.QualifiedItemId, queryResult.Item.Stack, queryResult.Item.Quality);
                //parsedItem.Stack *= multiplier;

                result.Add(parsedItem.QualifiedItemId, (drop.Chance, Game1.random.Next(drop.MinStack, drop.MaxStack)));
            }
        }

        return result;
    }
    
    public bool GetExtraResourceData(string id, bool isClump, out bool bombImmunity, out Enum resourceType)
    {
        bombImmunity = false;
        resourceType = CustomResourceType.Stone;

        if (string.IsNullOrWhiteSpace(id))
            return false;
        
        if (!isClump && ModEntry.Ores.TryGetValue(id, out var node))
        {
            bombImmunity = node.ImmuneToBombs;
            resourceType = node.Type;
            return true;
        }

        if (ModEntry.BigClumps.TryGetValue(id, out var clump))
        {
            bombImmunity = clump.ImmuneToBombs;
            resourceType = clump.Type;
            return true;
        }
        return false;
    }

    public bool GetResourceData(string id, bool isClump, out object data)
    {
        data = null;

        if (string.IsNullOrWhiteSpace(id))
            return false;
        
        if (!isClump && ModEntry.Ores.TryGetValue(id, out var node))
        {
            data = node;
            return true;
        }

        if (ModEntry.BigClumps.TryGetValue(id, out var clump))
        {
            data = clump;
            return true;
        }
        return false;
    }
}
