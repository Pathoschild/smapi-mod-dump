/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Contained;
using StardewModdingAPI;
using StardewValley;

namespace ItemExtensions.Models;

public class FarmerAnimation
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal FarmerSprite.AnimationFrame[] ActualAnimation { get; set; }
        
    /// <summary>
    /// In file: int[] of "frame duration" (repeat).
    /// In item: Can be a name (vanilla or custom), or animation frames.
    /// </summary>
    public FarmerFrame[] Animation { get; set; } = null;

    /// <summary>
    /// For custom items using pre-made animations.
    /// </summary>
    public string AnimateFrom { get; set; } = null;

    public FoodAnimation Food { get; set; } = null;
    
    /// <summary>
    /// Mutually exclusive with <see cref="Food"/>.
    /// </summary>
    public bool HideItem { get; set; } = false;

    public int Emote { get; set; } = -1;
    
    /// <summary>
    /// Message to show.
    /// </summary>
    public string ShowMessage { get; set; } = null;
    
    /// <summary>
    /// Sound to play.
    /// </summary>
    public string PlaySound { get; set; } = null;

    /// <summary>
    /// Optional sound delay.
    /// </summary>
    public int SoundDelay { get; set; } = 0;

    /// <summary>
    /// Track to change music to.
    /// </summary>
    public string PlayMusic { get; set; } = null;
    
    /// <summary>
    /// Only usable via item data.
    /// </summary>
    public string TriggerAction { get; set; } = null;

    internal bool IsValid(out FarmerAnimation result)
    {
        result = null;
        
        if ((Food is not null || Food != new FoodAnimation()) && HideItem)
        {
            Log("Eating animation and HideItem are mutually exclusive.", LogLevel.Warn);
            return false;
        }

        List<FarmerSprite.AnimationFrame> realFrames = new();
        if (Animation is null || Animation == Array.Empty<FarmerFrame>())
        {
            Log("Farmer frames aren't valid. Skipping", LogLevel.Warn);
            return false;
        }
   
        foreach (var farmer in Animation)
        {
            realFrames.Add(new FarmerSprite.AnimationFrame(farmer.Frame, farmer.Duration, farmer.SecondaryArm, farmer.Flip, farmer.HideArm));
        }
        ActualAnimation = realFrames.ToArray();
        result = this;

        return true;
    }
    /*
    internal bool IsValid(out FarmerAnimation result)
    {
        result = null;
        
        if ((Food is not null || Food != new FoodAnimation()) && HideItem)
        {
            Log("Eating animation and HideItem are mutually exclusive.", LogLevel.Warn);
            return false;
        }

        List<FarmerSprite.AnimationFrame> realFrames = new();
        
        switch (Animation)
        {
            case string s when string.IsNullOrWhiteSpace(s):
                Log("Must specify an animation.", LogLevel.Warn);
                return false;
            case string s:
                //divides by space, assuming it's of format "frame duration frame duration (...)"
                //every second check is skipped, as it'd be a duration value and not a frame
                try
                {
                    var parsed = ArgUtility.SplitBySpace(s);
                    var toInt = parsed.Select(int.Parse).ToList();
                    var skipNext = false;

                    for (var i = 0; i < toInt.Count - 1; i++)
                    {
                        if (skipNext)
                        {
                            skipNext = false;
                            continue;
                        }

                        realFrames.Add(new FarmerSprite.AnimationFrame(toInt[i], toInt[i + 1]));
                        skipNext = true;
                    }

                    ActualAnimation = realFrames.ToArray();
                    result = this;

                    return true;
                }
                catch (Exception e)
                {
                    Log($"Error: {e}", LogLevel.Error);
                    return false;
                }
            case string[] animation:
            {
                //assumes every string is of format "<frame> [duration] [secondaryArm] [flip] [hideArm]"
                foreach (var frames in animation)
                {
                    var parsed = ArgUtility.SplitBySpace(frames);
                    var toInt = parsed.Select(int.Parse).ToList();
                    var hasValue = ArgUtility.TryGetInt(parsed, 0, out var frame, out var error);
                    ArgUtility.TryGetOptionalInt(parsed, 1, out var duration, out _, 200);
                    ArgUtility.TryGetOptionalBool(parsed, 2, out var secondaryArm, out _);
                    ArgUtility.TryGetOptionalBool(parsed, 3, out var flip, out _);
                    ArgUtility.TryGetOptionalBool(parsed, 4, out var hideArm, out _);

                    if (!hasValue)
                    {
                        Log($"Error while parsing animation: {error}", LogLevel.Error);
                        return false;
                    }
                    
                    realFrames.Add(new FarmerSprite.AnimationFrame(frame, duration, secondaryArm, flip, hideArm));
                }
                ActualAnimation = realFrames.ToArray();
                result = this;

                return true;
            }
            default:
            {
                //if neither, assumes it's FarmerFrame
                if (Animation is null)
                {
                    Log("Farmer frames aren't valid. Skipping", LogLevel.Warn);
                    return false;
                }

                var farmerFrames = Animation as FarmerFrame[];
                
                if (farmerFrames is null)
                    return false;
            
                foreach (var farmer in farmerFrames)
                {
                    realFrames.Add(new FarmerSprite.AnimationFrame(farmer.Frame, farmer.Duration, farmer.SecondaryArm, farmer.Flip, farmer.HideArm));
                }
                ActualAnimation = realFrames.ToArray();
                result = this;

                return true;
            }
        }
    }*/
    public int TotalTime()
    {
        var time = 0;
        foreach (var frame in Animation)
        {
            time += frame.Duration;
        }

        return time;
    }
}