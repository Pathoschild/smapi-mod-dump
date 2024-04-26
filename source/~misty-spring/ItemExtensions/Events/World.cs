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
using ItemExtensions.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace ItemExtensions.Events;

public static class World
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static void ObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
        if(!e.Added.Any())
            return;

        foreach (var pair in e.Added)
        {
            if (ModEntry.Ores.TryGetValue(pair.Value.ItemId, out var resource) == false)
                continue;

            if (resource is null)
                continue;
            
            if(GeneralResource.IsVanilla(pair.Value.ItemId))
                continue;
            
            Log("Found data...");

            SetSpawnData(pair.Value, resource);
        }
    }

    internal static void SetSpawnData(Object o, ResourceData resource)
    {
        o.MinutesUntilReady = resource.Health;
        
        if (o.tempData is null)
        {
            o.tempData =  new Dictionary<string, object>
            {
                { "Health", resource.Health }
            };
        }
        else
        {
            o.tempData.TryAdd("Health", resource.Health);
        }

        o.modData["Esca.FarmTypeManager/CanBePickedUp"] = "false";
        
        if(o.tempData.ContainsKey(ModKeys.IsFtm) == false)
            o.modData[ModKeys.Days] = "0";
        
        o.CanBeSetDown = true;
        o.CanBeGrabbed = false;
        o.IsSpawnedObject = false;
        
        o.initializeLightSource(o.TileLocation);
    }
}