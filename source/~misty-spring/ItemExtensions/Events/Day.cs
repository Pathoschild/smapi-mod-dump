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

namespace ItemExtensions.Events;

public class Day
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
        }

        return;
        
        bool LoadCustomClumps(GameLocation arg)
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