/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Objects;

namespace ItemExtensions.Additions;

public static class Queries
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    public static bool ToolUpgrade(string[] query, GameStateQueryContext context)
    {
        //<q> <player> <tool> [min] [max] [recursive]

        var p = !ArgUtility.TryGet(query, 1, out var playerKey, out var error);
        var t = !ArgUtility.TryGet(query, 2, out var tool, out error);
        ArgUtility.TryGetOptionalInt(query, 3, out var min, out _, defaultValue: 0);
        ArgUtility.TryGetOptionalInt(query, 4, out var max, out _, defaultValue: 4);
        ArgUtility.TryGetOptionalBool(query, 5, out var recursive, out _);
        var invalid = p || t ;
        
        //Log($"Got called (ToolUpgrades): {playerKey} {tool} {min} {max}");
        
        return invalid ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, target =>
        {
            var toolItem = target.getToolFromName(tool);

            if (toolItem == null && recursive)
            {
                var isAnyPlayer = !playerKey.Equals("Any", StringComparison.OrdinalIgnoreCase);
                toolItem = RecursiveToolSearch(tool, isAnyPlayer, target);
            }

            //if we found no matching tool
            if (toolItem is null)
                return false;
            
            Log($"data: target {target.displayName}, tool {toolItem.DisplayName} ({toolItem.UpgradeLevel}), min required {min}");
            
            var upgrade = toolItem.UpgradeLevel;
            return upgrade >= min && upgrade <= max;
        });
    }

    private static Tool RecursiveToolSearch(string name, bool matchOwner = false, Farmer who = null)
    {
        if (matchOwner && who == null)
            throw new ArgumentException("When using matchOwner, who(Farmer) can't be null.", nameof(who));

        Tool result = null;
        var flag = true;
        
        Utility.ForEachLocation(l =>
        {
            foreach(var pair in l.Objects.Pairs)
            {
                var obj = pair.Value;

                // chests
                if (obj is Chest chest && chest.playerChest.Value)
                {
                    chest.ForEachItem(HandleMatch);
                }
            }

            return flag;
        }, true, true);

        return result;

        bool HandleMatch(Item item, Action remove, Action<Item> replaceWith)
        {
            if (item is not Tool t) 
                return true;

            if (matchOwner && t.getLastFarmerToUse() != who)
                return true;
            
            if (t.Name != name)
                return true;
            
            result = t;
            flag = false;
            return false;
        }
    }

    /// <summary>
    /// Uses custom parameters (min, max, quality) to make an item search in inventory.
    /// </summary>
    /// <param name="query">Arguments.</param>
    /// <param name="context">Query context.</param>
    /// <returns>Whether item exists.</returns>
    public static bool InInventory(string[] query, GameStateQueryContext context)
    {
        //Format: query WHO qualifiedID min max minquality

        //First get all arguments
        var noPlayer = !ArgUtility.TryGet(query, 1, out var playerKey, out var error);
        var noId = !ArgUtility.TryGet(query, 2, out var id, out error);
        ArgUtility.TryGetOptionalInt(query, 3, out var min, out _);
        ArgUtility.TryGetOptionalInt(query, 4, out var max, out _, defaultValue: -1);
        ArgUtility.TryGetOptionalInt(query, 4, out var minQuality, out _, defaultValue: 0);
        
        //if no id or player, return false and error
        var invalid = noId || noPlayer;
        
        return invalid ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, target =>
        {
            var hasId = target.Items.ContainsId(id, min);
            
            //if no id or no minimum
            if(!hasId)
                return false;

            //if we have item, & no max nor quality set
            if (max <= 0 && minQuality <= 0)
                return true;

            //otherwise check max
            var items = target.Items.GetById(id);
            var enumerable = items.ToList();
            
            if (!enumerable.Any())
                return false;

            //get item count across all inventory, considering quality
            var num = enumerable.Where(item => item.Quality >= minQuality).Sum(item => item.Stack);

            //using quality-aware(?) count:
            
            //if no max, check whether bigger than minimum
            if (max <= 0)
                return num > min;
            
            //otherwise, check if its less or equals max
            return num <= max;
        });
    }
}