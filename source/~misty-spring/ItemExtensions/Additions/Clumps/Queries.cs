/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using ItemExtensions.Models;
using ItemExtensions.Models.Enums;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Internal;

namespace ItemExtensions.Additions.Clumps;

public static class ClumpQueries
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

    public static string Resolve(string id, string perItemConditions, ItemQueryContext context)
    {
        try
        {
#if DEBUG
            Log($"id: {id}, peritemconditions: {perItemConditions}");
#endif
            string clump;
            
            if (id.StartsWith(ModKeys.AllClumpsForage))
            {
                var trimmed = id.Replace(ModKeys.AllClumpsForage, "");
                clump = AllClumps(ModKeys.AllClumpsForage, trimmed, perItemConditions, context);
            }
            else
            {
                var trimmed = id.Replace(ModKeys.RandomClumpForage, "");
                clump = RandomClump(ModKeys.RandomClumpForage, trimmed, perItemConditions, context);
            }
#if DEBUG
            Log($"Result: {clump}");
#endif
            return clump;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            return null;
        }
    }

    private static string RandomClump(string key, string arguments, string perItemCondition, ItemQueryContext context)
    {
        try
        {
            var random = context.Random ?? Game1.random;
            return random.ChooseFrom(SolveQuery(key, arguments, perItemCondition, context));
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            return null;
        }
    }

    /// <summary>
    /// Returns all clumps. See <see cref="ItemQueryResolver.DefaultResolvers.ALL_ITEMS"/>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="arguments"></param>
    /// <param name="perItemCondition"></param>
    /// <param name="context"></param>
    /// <returns>All clumps that meet conditions.</returns>
    internal static string AllClumps(string key, string arguments, string perItemCondition, ItemQueryContext context)
    {
        /* ALL_CLUMPS [flags] */
        try
        {
            return SolveQuery(key, arguments, perItemCondition, context).FirstOrDefault();
        }
        catch (Exception e)
        {
            Log($"Error: {e}");
            return null;
        }
    }

    private static List<string> SolveQuery(string key, string arguments, string perItemCondition, ItemQueryContext context)
    {
        var results = new List<string>();

        if (string.IsNullOrWhiteSpace(arguments))
            arguments = "null";

        List<string> clumpPreconditions = new();
        List<string> trueConditions = new();

        if (!string.IsNullOrWhiteSpace(perItemCondition))
        {
            //do this to avoid bugs - remove space after comma if exists
            var parsedPerItem = perItemCondition.Replace(", ", ",");

            var conditions = ArgUtility.SplitQuoteAware(parsedPerItem,',');
            foreach (var condition in conditions)
            {
                //if not "any" condition we parse directly
                if (!condition.StartsWith("ANY", IgnoreCase))
                {
                    //if valid clump condition
                    if (condition.StartsWith("ITEM_ID", IgnoreCase) || condition.StartsWith("ITEM_NUMERIC_ID", IgnoreCase) || condition.StartsWith("ITEM_CONTEXT_TAG", IgnoreCase))
                    {
                        clumpPreconditions.Add(condition);
                    }
                    //if any other id type
                    else if (condition.StartsWith("ITEM_"))
                    {
                        //ignore if any other item query
                        continue;
                    }
                    //else, add to true conditions
                    else
                    {
                        trueConditions.Add(condition);
                    }

                    //so we don't go into ANY's code
                    continue;
                }
                
                // but, if "ANY":
                //split by comma, quote aware (although that shouldn't be a thing inside ANY, but JIC)
                var subsplit = ArgUtility.SplitQuoteAware(condition.Remove(0,4), ',');
                //stringbuilder for 
                var forTrue = new StringBuilder("ANY ");
                var forClump= new StringBuilder("ANY ");
                //for each one inside
                foreach(var subsub in subsplit)
                {
                    //if item, add to this "Any" SB
                    if (subsub.StartsWith("ITEM_ID", IgnoreCase) || subsub.StartsWith("ITEM_NUMERIC_ID", IgnoreCase) ||
                        subsub.StartsWith("ITEM_CONTEXT_TAG", IgnoreCase))
                    {
                        forClump.Append('\"');
                        forClump.Append(subsub);
                        forClump.Append('\"');
                        forClump.Append(',');
                    }
                    //if any other id type ignore
                    else if (subsub.StartsWith("ITEM_"))
                    {
                        //ignore if any other item query
                        continue;
                    }
                    //otherwise, add to "true conditions" one
                    else
                    {
                        forTrue.Append('\"');
                        forTrue.Append(subsub);
                        forTrue.Append('\"');
                        forTrue.Append(',');
                    }
                }
                //remove last & add
                if (forTrue.ToString() != "ANY ")
                {
                    forTrue.Remove(forTrue.Length - 1, 1);
                    trueConditions.Add(forTrue.ToString());
                }
                if (forClump.ToString() != "ANY ")
                {
                    forClump.Remove(forTrue.Length - 1, 1);
                    clumpPreconditions.Add(forTrue.ToString());
                }
            }
        }

        var generalConditions = TurnIntoCondition(trueConditions);    //can be parsed safely
        
        var array = ItemQueryResolver.Helpers.SplitArguments(arguments);
        ResolveArguments(key, array, out var tool, out var skill, out var statCounter, out var width, out var height, out var hasLight, out var hasExtraDrops, out var secretNotes, out var addsHay);

        var weapons = new[]{ "meleeweapon", "weapon", "dagger", "club", "hammer", "sword", "slash", "slashing", "slashing sword", "slashingsword", "stabbing sword", "stabbingsword" };
        
        foreach (var (id, clump) in ModEntry.BigClumps)
        {
            var flag = true;
            //first solve perItemCondition
            foreach (var condition in clumpPreconditions)
            {
                #if DEBUG
                Log("Checking clump condition: " + condition);
                #endif
                
                if (condition.StartsWith("ANY"))
                {
                    //if any subconditions are met
                    var flag2 = false;
                    var rem = condition.Remove(0, 4);
                    
                    //check each
                    foreach (var str in rem.Split(','))
                    {
                        //if not met, continue checking
                        if (TrySolve(str, id, clump) == false)
                            continue;
                        
                        //if found, stop check
                        flag2 = true;
                        break;
                    }

                    if (flag2 == false)
                    {
                        flag = false;
                        break;
                    }
                }
                else
                {
                    flag = TrySolve(condition, id, clump);
                    
                    if (flag == false)
                        break;
                    /*if (condition.StartsWith("ITEM_NUMERIC_ID", IgnoreCase))
                    {
                        //implement at some point
                    }*/
                }
            }
            //if not met
            if(flag == false)
                continue;
            
            //if has conditions & not matched
            #if DEBUG
            Log($"General conditions: {generalConditions}");
            #endif

            if(string.IsNullOrWhiteSpace(generalConditions) == false && GameStateQuery.CheckConditions(generalConditions, context.Location, Game1.MasterPlayer, random: context.Random) == false)
                continue;
            
            //if has tool AND tool isn't "any"
            if (!string.IsNullOrWhiteSpace(tool) && tool.Equals("any", IgnoreCase) == false)
            {
#if DEBUG
                Log($"Checking tool: {tool}");
#endif
                //if a weapon
                if (tool.Equals("weapon", IgnoreCase))
                {
                    //if int
                    if (int.TryParse(clump.Tool, out var number))
                    {
                        if(number < 0 || number > 3)
                            continue;
                    }
                    //if target tool isn't weapon
                    else if(weapons.Contains(clump.Tool.ToLower()) == false)
                        continue;
                }
                
                //if not, compare
                if(tool.Equals(clump.Tool, IgnoreCase) == false)
                    continue;
            }
            //if skill requirement (-2 any, otherwise specific)
            if (skill != -1)
            {
#if DEBUG
                Log($"Checking skill: {skill}");
#endif
                //if any and clump DOESN'T have skill
                if(skill == -2 && clump.ActualSkill <= -1)
                    continue;
                //otherwise compare them
                else if(skill != clump.ActualSkill)
                    continue;
            }
            
            if (statCounter.HasValue && statCounter != clump.CountTowards)
            {
#if DEBUG
                Log($"statCounter: {statCounter} not met.");
#endif
                continue;
            }

            if (height.Item1 > 1)
            {
#if DEBUG
                Log($"Checking height: {height}");
#endif
                //if doesnt reach minimum
                if (height.Item1 >  clump.Height)
                    continue;
                //if passes maximum
                if(height.Item2 < clump.Height)
                    continue;
            }
            
            if (width.Item1 > 1)
            {
#if DEBUG
                Log($"Checking width: {width}");
#endif
                //if doesnt reach minimum
                if (width.Item1 >  clump.Width)
                    continue;
                //if passes maximum
                if(width.Item2 < clump.Width)
                    continue;
            }
            
            if (hasLight.HasValue)
            {
#if DEBUG
                Log($"hasLight: {hasLight}");
#endif
                if (hasLight.Value == true && clump.Light == null)
                    continue;
                if(hasLight.Value == false && clump.Light != null)
                    continue;
            }
            
            if (hasExtraDrops.HasValue)
            {
#if DEBUG
                Log($"hasExtraDrops: {hasExtraDrops}");
#endif
                if (hasExtraDrops.Value == true && clump.ExtraItems == null)
                    continue;
                if(hasExtraDrops.Value == false && clump.ExtraItems != null)
                    continue;
            }
            
            if (secretNotes is true && clump.SecretNotes == false)
            {
#if DEBUG
                Log($"secretNotes: {secretNotes} not met.");
#endif
                continue;
            }
            
            if (addsHay.Item1 >= 0)
            {
#if DEBUG
                Log($"Checking addsHay: {addsHay}");
#endif
                //if doesnt reach minimum
                if (addsHay.Item1 >  clump.AddHay)
                    continue;
                //if passes maximum
                if(addsHay.Item2 < clump.AddHay)
                    continue;
            }
            
            results.Add(id);
        }

        return results;
    }

    private static bool TrySolve(string condition, string id, ResourceData clump)
    {
        if (condition.StartsWith("ITEM_CONTEXT_TAG", IgnoreCase))
        {
            var tag = condition.Split(' ')[2];
            return clump.ContextTags.Contains(tag);
        }
        else if (condition.StartsWith("ITEM_ID_PREFIX", IgnoreCase))
        {
            var thisId = condition.Split(' ')[2];
            return id.StartsWith(thisId);
        }
        else if (condition.StartsWith("ITEM_ID", IgnoreCase))
        {
            var thisId = condition.Split(' ')[2];
            return id.Equals(thisId);
        }

        return true;
    }

    private static string TurnIntoCondition(List<string> conditions)
    {
        if (conditions is null || conditions.Any() == false)
            return null;
        
        StringBuilder result = new();
        foreach (var s in conditions)
        {
            result.Append(s);
            result.Append(',');
        }

        result.Remove(result.Length - 1, 1);

        return result.ToString();
    }

    private static void ResolveArguments(string key, string[] array, out string tool, out int skill, out StatCounter? statCounter, out (int,int) width, out (int,int) height, out bool? hasLight, out bool? hasExtraDrops,  out bool? secretNotes, out (int,int)  addsHay)
    { 
        /*
         * flags:
         * - @tool:TYPE
         * - @skill:TYPE
         * - @stat:TYPE
         * - @width:min_max
         * - @height:min_max
         * - @hasLight
         * - @hasExtraDrops         //if accompanied of a :, checks ids separately
         * - @addsHay
         * - @secretNotes
         */
        
        tool = null;
        skill = -1;
        statCounter = null;
        width = (0, 999);
        height = (0, 999);
        hasLight = null;
        hasExtraDrops = null;
        addsHay = (-1, 999);
        secretNotes = null;
        
        //for parsing
        string[] minmax;
        int min;
        int max;
        
        
        for (var index = 0; index < array.Length; ++index)
        {
          var str = array[index];
          //light
          if (str.Equals("@hasLight", IgnoreCase))
          {
              hasLight = true;
          }
          else if (str.Equals("@noLight", IgnoreCase))
          {
              hasLight = false;
          }
          //has drops
          else if (str.Equals("@hasExtraDrops", IgnoreCase))
          {
              hasExtraDrops = true;
          }
          else if (str.Equals("@noExtraDrops", IgnoreCase))
          {
              hasExtraDrops = false;
          }
          //has hay
          else if (str.Equals("@addsHay", IgnoreCase))
          {
              addsHay = (1, 999);
          }
          //specific hay
          else if (str.StartsWith("@addsHay:", IgnoreCase))
          {
              minmax = str.Remove(0,9).Split('_');
              ArgUtility.TryGetInt(minmax, 0, out min, out _);
              ArgUtility.TryGetOptionalInt(minmax, 1, out max, out _, 999);
              addsHay = (min, max);
          }
          //has notes
          else if (str.Equals("@secretNotes", IgnoreCase))
          {
              secretNotes = true;
          }
          //has skill
          else if (str.Equals("@skill", IgnoreCase))
          {
              skill = -2; //must have any
          }
          else if (str.StartsWith("@skill:", IgnoreCase))
          {
              var skillType = str.Remove(0,7);
              skill = ResourceData.GetSkill(skillType);
          }
          //this tool type
          else if (str.StartsWith("@tool:", IgnoreCase))
          {
              var tooltype = str.Remove(0,6);
              tool = tooltype;
          }
          //any stat
          else if (str.Equals("@statcounter", IgnoreCase))
          {
              statCounter = StatCounter.Any;
          }
          //specific stat
          else if (str.StartsWith("@statcounter:", IgnoreCase))
          {
              var trimmed = str.Remove(0, 13);
              if(Enum.TryParse<StatCounter>(trimmed, out var stat))
                  statCounter = stat;
          }
          else if (str.StartsWith("@width:", IgnoreCase))
          {
              minmax = str.Remove(0,7).Split('_');
              ArgUtility.TryGetInt(minmax, 0, out min, out _);
              ArgUtility.TryGetOptionalInt(minmax, 1, out max, out _, 999);
              width = (min, max);
          }
          else if (str.StartsWith("@height:", IgnoreCase))
          {
              minmax = str.Remove(0,8).Split('_');
              ArgUtility.TryGetInt(minmax, 0, out min, out _);
              ArgUtility.TryGetOptionalInt(minmax, 1, out max, out _, 999);
              height = (min, max);
          }
          else if (str.StartsWith('@'))
          {
              throw new ArgumentException($"{key}: index {index} has unknown option flag '{str}'");
          }
        }
    }
}