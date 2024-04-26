/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using ItemExtensions.Additions.Clumps;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ItemExtensions.Patches;

public class TractorModPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        var updateAttachments = AccessTools.Method($"Pathoschild.Stardew.TractorMod.Framework.TractorManager:UpdateAttachmentEffects");

        if (updateAttachments is null) //if the method isn't found, return
        {
            Log($"Method not found. (UpdateAttachmentEffects)", LogLevel.Warn);
            return;
        }
        
        Log($"Applying Harmony patch \"{nameof(TractorModPatches)}\": postfixing mod method \"Pathoschild.Stardew.TractorMod.Framework.TractorManager:UpdateAttachmentEffects\".");
        
        harmony.Patch(
            original: updateAttachments,
            postfix: new HarmonyMethod(typeof(TractorModPatches), nameof(Post_UpdateAttachmentEffects))
        );
    }

    private static void Post_UpdateAttachmentEffects()
    {
        // get context
        var player = Game1.player;
        var location = player.currentLocation;
        var tool = player.CurrentTool;

        if (tool is null)
            return;

        if (GetRange(out var distance) == false)
            return;
        
        var grid = GetTileGrid(Game1.player.Tile, distance).ToArray();
        foreach (var tile in grid)
        {
#if DEBUG
            Log("Tile: " + tile, LogLevel.Info);
#endif   
            var obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);
            if (obj is not null && ModEntry.Ores.ContainsKey(obj.ItemId))
            {
                obj.performToolAction(tool);
            }
            
            var clumps = location.resourceClumps?.Where(s => s.occupiesTile((int)tile.X, (int)tile.Y));
            
            if (clumps is null)
                return;
            
            foreach (var resource in clumps)
            {
                if(ExtensionClump.IsCustom(resource))
                    resource.performToolAction(tool, tool.UpgradeLevel, player.Tile);
            }
        }
    }

    private static bool GetRange(out int amount)
    {
        amount = -1;
        
        var tractorMod = ModEntry.Help.ModRegistry.Get("Pathoschild.TractorMod");
        var mod = (IMod)tractorMod?.GetType()?.GetProperty("Mod")?.GetValue(tractorMod);
        
        if (mod is null)
            return false;
        
        var config = mod.GetType()?.GetField("Config", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod);
        if (config?.GetType() is null)
            return false;
        
        var distance = config.GetType().GetProperty("Distance");
        if (distance is null)
            return false;

        try
        {
            amount = (int)distance?.GetValue(config);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static IEnumerable<Vector2> GetTileGrid(Vector2 origin, int distance)
    {
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
                yield return new Vector2(origin.X + x, origin.Y + y);
        }
    }
}