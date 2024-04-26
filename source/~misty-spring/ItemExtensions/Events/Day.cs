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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace ItemExtensions.Events;

public static class Day
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    /// <summary>
    /// On day start, set each clump's custom texture and index.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void Started(object sender, DayStartedEventArgs e)
    {
        List<ResourceClump> clumps = new();
        
        Utility.ForEachLocation(LoadCustomClumps, true, true);
        Utility.ForEachLocation(CheckNodeDuration, true, true);

        foreach (var resource in clumps)
        {
            //if no custom id (shouldn't happen but JIC)
            if (resource.modData.TryGetValue(ModKeys.ClumpId, out var id) == false)
            {
                Log($"Clump at {resource.Location.NameOrUniqueName} {resource.Tile} doesn't seem to have mod data.");
                continue;
            }
            
            //if not found in data
            if(ModEntry.BigClumps.TryGetValue(id, out var data) == false)
            {
                Log($"Couldn't find mod data for custom clump {id}. Resource will stay as default clump.", LogLevel.Info);
                continue;
            }

            //if texture not found
            if (Game1.content.DoesAssetExist<Texture2D>(data.Texture))
            {
                resource.textureName.Set(data.Texture);
            }
            else
            {
                Log($"Couldn't find texture {data.Texture} for clump at {resource.Location.NameOrUniqueName} {resource.Tile}. Resource will stay as default clump.", LogLevel.Info);
                continue;
            }
            
            resource.parentSheetIndex.Set(data.SpriteIndex);
            resource.loadSprite();

            if (resource.modData.TryGetValue(ModKeys.Days, out var dayString) && int.TryParse(dayString, out var days))
            {
                resource.modData[ModKeys.Days] = $"{days + 1}";
            }
        }

        return;
        
        bool LoadCustomClumps(GameLocation arg)
        {
            string removeAfter = null;
            var howLong = 0;
            var needsRemovalCheck = arg?.GetData()?.CustomFields != null && arg.GetData().CustomFields.TryGetValue(ModKeys.ClumpRemovalDays, out removeAfter);
            
            if (needsRemovalCheck)
                int.TryParse(removeAfter, out howLong);
            
            var removalQueue = new List<ResourceClump>();
            
            foreach (var resource in arg.resourceClumps)
            {
                //if not custom
                if(resource.modData.ContainsKey(ModKeys.ClumpId) == false)
                    continue;

                if (needsRemovalCheck && resource.modData.TryGetValue(ModKeys.Days, out var daysSoFar) &&
                    int.TryParse(daysSoFar, out var days))
                {
                    if (howLong <= days)
                    {
                        removalQueue.Add(resource);
                        continue;
                    }
                }
                
                clumps.Add(resource);
            }

            //remove all that have more days than allowed
            foreach (var resourceClump in removalQueue)
            {
                arg.resourceClumps.Remove(resourceClump);
            }
            
            return true;
        }
    }

    /// <summary>
    /// For each location, check forage duration.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns>Whether to keep iterating code, or stop.</returns>
    private static bool CheckNodeDuration(GameLocation arg)
    {
        if (arg is null)
            return true;
        
        string removeAfter = null;
        int howLong;
        var needsRemovalCheck = arg.GetData()?.CustomFields != null && arg.GetData().CustomFields.TryGetValue(ModKeys.NodeRemovalDays, out removeAfter);
            
        //if there's property, parse. otherwise ignore location
        if (needsRemovalCheck)
        {
            //if couldn't parse days
            if (int.TryParse(removeAfter, out howLong) == false)
            {
                Log($"Couldn'y parse NodeRemovalDays property for location {arg.DisplayName} ({arg.NameOrUniqueName}). Skipping", LogLevel.Info);
                return true;
            }
        }
        else
        {
            return true;
        }
            
        var removalQueue = new List<Object>();
        
        //for each object, do check
        foreach (var obj in arg.Objects.Values)
        {
            if (obj.modData is null)
                continue;
            
            if (obj.modData.TryGetValue(ModKeys.IsFtm, out var ftm) && bool.Parse(ftm))
                continue;

            if (obj.modData.TryGetValue(ModKeys.Days, out var daysSoFar) == false || int.TryParse(daysSoFar, out var days) == false) 
                continue;
            
            obj.modData[ModKeys.Days] = $"{days + 1}";
            
            if (howLong > days) 
                continue;
            
            removalQueue.Add(obj); ;
        }

        //remove all that have more days than allowed
        foreach (var obj2 in removalQueue)
        {
            arg.Objects.Remove(obj2.TileLocation);
        }

        return true;
    }

    /// <summary>
    /// On day ending, set all custom clumps to a default stone. This is made in case the mod (or a component) is uninstalled.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void Ending(object sender, DayEndingEventArgs e)
    {
        List<ResourceClump> clumps = new();
        
        Utility.ForEachLocation(CheckForCustomClumps, true, true);

        foreach (var resource in clumps)
        {
            //give default values of a stone
            resource.textureName.Set("Maps/springobjects");
            resource.parentSheetIndex.Set(672);
            resource.loadSprite();
        }

        return;

        bool CheckForCustomClumps(GameLocation arg)
        {
            foreach (var resource in arg.resourceClumps)
            {
                //if not custom
                if(resource.modData.ContainsKey(ModKeys.ClumpId) == false)
                    continue;
                    
                clumps.Add(resource);
            }

            return true;
        }
    }
}