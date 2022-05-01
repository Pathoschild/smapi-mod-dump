/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from displaying their birthdays on calendars.</summary>
    public static class HarmonyPatch_BirthdayCalendar
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BirthdayCalendar)}\": transpiling SDV constructor \"Billboard(bool)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Constructor(typeof(Billboard), new[] { typeof(bool) }),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_BirthdayCalendar), nameof(Billboard_Constructor))
            );
        }

        /// <summary>Inserts a call to <see cref="IncludeBirthday"/> when the constructor chooses birthdays to display.</summary>
        /// <remarks>
        /// Old C#:
        ///     if (k.isVillager() && k.Birthday_Season != null && k.Birthday_Season.Equals(Game1.currentSeason) && ...
        ///     
        /// New C#:
        ///     if (k.isVillager() && IncludeBirthday(k) && k.Birthday_Season != null && k.Birthday_Season.Equals(Game1.currentSeason) && ...
        ///     
        /// Old IL:
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.Boolean StardewValley.NPC::isVillager()
        ///     brfalse Label5
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.String StardewValley.NPC::get_Birthday_Season()
        ///     brfalse Label6
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.String StardewValley.NPC::get_Birthday_Season()
        ///     ldsfld System.String StardewValley.Game1::currentSeason
        ///     callvirt virtual System.Boolean System.String::Equals(System.String value)
        ///     brfalse.s Label7
        ///     
        /// New IL:
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.Boolean StardewValley.NPC::isVillager()
        ///     brfalse Label5
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     call static System.Boolean CustomNPCExclusions.HarmonyPatch_BirthdayCalendar::IncludeBirthday(StardewValley.NPC npc)
        ///     brfalse Label6
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.String StardewValley.NPC::get_Birthday_Season()
        ///     brfalse Label6
        ///     ldloc.s 4 (StardewValley.NPC)
        ///     callvirt System.String StardewValley.NPC::get_Birthday_Season()
        ///     ldsfld System.String StardewValley.Game1::currentSeason
        ///     callvirt virtual System.Boolean System.String::Equals(System.String value)
        ///     brfalse.s Label7
        /// </remarks>
        public static IEnumerable<CodeInstruction> Billboard_Constructor(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo getBirthdaySeason = AccessTools.PropertyGetter(typeof(NPC), nameof(NPC.Birthday_Season)); //get info for a nearby method (the Birthday_Season property's "get" method)
                MethodInfo includeBirthday = AccessTools.Method(typeof(HarmonyPatch_BirthdayCalendar), nameof(IncludeBirthday)); //get the new method's info

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 3; x >= 0; x--) //for each instruction (looping backward & leaving space for multi-entry checks)
                {
                    if ((patched[x + 1].operand as MethodInfo) == getBirthdaySeason //if x+1 is calling Birthday_Season
                        && patched[x + 2].opcode == OpCodes.Brfalse) //AND x+2 is "branch if false"
                    {
                        var new0 = patched[x].Clone(); //copy x
                        var new1 = new CodeInstruction(OpCodes.Call, includeBirthday); //create a call to the new method
                        var new2 = patched[x + 2].Clone(); //copy x+2
                        patched.InsertRange(x, new[] { new0, new1, new2 }); //insert these new instructions before the Birthday_Season check
                    }
                }

                return patched;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BirthdayCalendar)}\" has encountered an error. Transpiler \"{nameof(Billboard_Constructor)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>Checks whether the given NPC's birthday should be displayed on calendars.</summary>
        /// <returns>True if the NPC's birthday should be included on calendars. False if it should be excluded.</returns>
        public static bool IncludeBirthday(NPC npc)
        {
            List<string> excluded = ModEntry.GetNPCExclusions(npc.Name, false); //get exclusion data for this NPC

            if (excluded.Exists(entry =>
                entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if this NPC is excluded from everything
                || entry.StartsWith("TownEvent", StringComparison.OrdinalIgnoreCase) //OR if this NPC is excluded from town events
                || entry.StartsWith("Calendar", StringComparison.OrdinalIgnoreCase) //OR this NPC is excluded from birthday calendars
            ))
            {
                ModEntry.Instance.Monitor.VerboseLog($"Excluded NPC's birthday from a calendar: {npc.Name}");
                return false; //exclude this NPC
            }

            return true; //allow this NPC
        }
    }
}