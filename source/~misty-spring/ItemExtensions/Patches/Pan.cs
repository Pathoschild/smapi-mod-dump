/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Tools;

namespace ItemExtensions.Patches;

public class PanPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(PanPatches)}\": postfixing mod method \"Pan.getPanItems\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Pan), nameof(Pan.getPanItems)),
            postfix: new HarmonyMethod(typeof(PanPatches), nameof(Post_getPanItems))
        );
    }
    
    public static void Post_getPanItems(Pan __instance, GameLocation location, Farmer who, List<Item> __result)
    {
        try
        {
            var context = new ItemQueryContext(location, who, Game1.random);

            foreach (var data in ModEntry.Panning)
            {
                if(__instance.UpgradeLevel < data.MinUpgrade || (data.MaxUpgrade > 0 && __instance.UpgradeLevel > data.MaxUpgrade))
                    continue;
                
                if (data.Chance < Game1.random.NextDouble())
                    continue;

                if (string.IsNullOrWhiteSpace(data.Condition) == false &&
                    GameStateQuery.CheckConditions(data.Condition) == false)
                    continue;

                var itemQueryResults = ItemQueryResolver.TryResolve(data, context, data.Filter, data.AvoidRepeat);

                //if none or result's item is null
                if (itemQueryResults.Count <= 0 || itemQueryResults.FirstOrDefault()?.Item is null)
                    continue;

                var item = itemQueryResults.FirstOrDefault()?.Item as Item;
                __result.Add(item);
            }
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}