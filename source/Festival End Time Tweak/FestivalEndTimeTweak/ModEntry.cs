using Harmony;
using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;

namespace FestivalEndTimeTweak
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Harmony patcher
            //https://github.com/kirbylink/FestivalEndTimeTweak.git
            var harmony = HarmonyInstance.Create("com.github.kirbylink.festivalendtimetweak");
            var original = typeof(Event).GetMethod("exitEvent");
            var prefix = helper.Reflection.GetMethod(typeof(FestivalEndTimeTweak.ChangeFestivalEndTime), "Prefix").MethodInfo;
            var postfix = helper.Reflection.GetMethod(typeof(FestivalEndTimeTweak.ChangeFestivalEndTime), "Postfix").MethodInfo;
            var transpiler = helper.Reflection.GetMethod(typeof(FestivalEndTimeTweak.ChangeFestivalEndTime), "Transpiler").MethodInfo;
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix), new HarmonyMethod(transpiler));
            
        }
    }

    public static class ChangeFestivalEndTime
    {
        /* Check if Event.isFestival is true before going into exitEven() */
        static void Prefix(Event __instance, ref bool __state)
        {
            if (__instance != null)
            {
                __state = __instance.isFestival;
            }
        }

        static void Postfix(Event __instance, ref bool __state)
        {
            /*  Current Code: Sets end of festival time to 2200 or 2400 depending on event
            *
            *   Change: Set festival end times to respective end time of each festival
            *   (Can change based on other mods but this code will handle festival xnb file changes)
            *   Spring 13th, Egg Festival:                      0900 - 1400
            *   Spring 24th, Flower Dance:                      0900 - 1400
            *   Summer 11th, Luau:                              0900 - 1400
            *   Summer 28th, Dance of the Moonlight Jellies:    2200 - 2400
            *   Fall 16th,   Stardew Valley Fair:               0900 - 1500
            *   Fall 27th,   Sprit's Eve:                       2200 - 2350
            *   Winter 8th,  Festival of Ice:                   0900 - 1400
            *   Winter 25th, Feast of the Winter Star:          0900 - 1400
            */

            if (__state)
            {
                var field = AccessTools.Field(typeof(Event), "festivalData");
                var festivalData = (Dictionary<string,string>) field.GetValue(__instance);
                int startTime = Convert.ToInt32(festivalData["conditions"].Split('/')[1].Split(' ')[0]);
                int endTime = Convert.ToInt32(festivalData["conditions"].Split('/')[1].Split(' ')[1]);
                int minutes = 60 * (endTime - startTime) / 100 ;
                Game1.timeOfDayAfterFade = endTime;
                
                foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
                {
                    foreach (StardewValley.Object @object in location.objects.Values)
                        @object.minutesElapsed(minutes, location);
                }
            }
        }

        /* Stop StardewValley.Object updates. Completed in Postfix */
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            MethodInfo minutesElapsedMI = typeof(StardewValley.Object).GetMethod("minutesElapsed", new Type[] { typeof(Int32), typeof(StardewValley.GameLocation) });
            for (var i = 2; i < codes.Count; i++)
            {
                if (codes[i].operand != null && codes[i].operand.GetType().IsInstanceOfType(typeof(MethodInfo)) && minutesElapsedMI.Equals((MethodInfo) codes[i].operand))
                {
                    codes[i - 2].opcode = OpCodes.Nop; //Nop minutesElapsed() param1 load
                    codes[i - 1].opcode = OpCodes.Nop; //Nop minutesElapsed() param2 load
                    codes[i].opcode = OpCodes.Nop;     //Nop minutesElapsed() method call
                    codes[i + 1].opcode = (codes[i + 1].opcode == OpCodes.Pop ? OpCodes.Nop : codes[i].opcode);
                    
                 
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
