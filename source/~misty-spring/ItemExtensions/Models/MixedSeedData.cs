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

namespace ItemExtensions.Models.Contained;

public class MixedSeedData
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static IModHelper Helper => ModEntry.Help;
    
    public string ItemId { get; set; }
    public string Condition { get; set; }
    public string HasMod { get; set; }
    public int Weight { get; set; } = 1;

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(ItemId))
        {
            Log("Must specify seed name! Skipping", LogLevel.Warn);
            return false;
        }

        var items = Game1.objectData;
        
        if (items.ContainsKey(ItemId) == false)
        {
            Log("Seed doesn't seem to exist in-game. (Logged for debugging purposes)");
        }

        return true;
    }

    public bool CheckConditions()
    {
        if (string.IsNullOrWhiteSpace(HasMod))
            return true;

        return Helper.ModRegistry.Get(HasMod) != null;
    }
}

public enum SeasonCondition{
    /// <summary> Any season. </summary>
    Any,
    /// <summary>The spring season.</summary>
    Spring,
    /// <summary>The summer season.</summary>
    Summer,
    /// <summary>The fall season.</summary>
    Fall,
    /// <summary>The winter season.</summary>
    Winter,
    /// <summary> Only if it's indoors.</summary>
    Indoors
}