using System;
using StardewModdingAPI;
using StardewValley;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace TwentyFourHours
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        // internal static IMonitor Logger;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // ModEntry.Logger = this.Monitor;
            var harmony = HarmonyInstance.Create("com.github.princeleto.twentyfourhours");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public static string ConvertTime(int time)
        {
            var hours = time / 100 % 24;
            var minutes = time % 100;
            var hoursText = (hours < 10 ? "0" : "") + hours.ToString();
            var minutesText = (minutes < 10 ? "0" : "") + minutes.ToString();
            return hoursText + ":" + minutesText;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                string pattern = @"([0-9]{1,2}):([0-9]{2})(AM|PM)";
                asset.AsDictionary<string, string>().Set((id, data) =>
                {
                    MatchCollection matches = Regex.Matches(data, pattern);
                    foreach (Match match in matches)
                    {
                        GroupCollection hop = match.Groups;
                        int hours = Int32.Parse(hop[1].ToString());
                        string minutes = hop[2].ToString();
                        string afternoon = hop[3].ToString();

                        if ((afternoon == "AM" && hours == 12) || (afternoon == "PM" && hours < 12))
                        {
                            hours = (hours + 12) % 24;
                        }

                        string hoursText = (hours < 10 ? "0" : "") + hours.ToString();
                        string result = hoursText + ":" + minutes;

                        var regex = new Regex(Regex.Escape(hop[0].ToString()));
                        data = regex.Replace(data, result, 1);
                    }
                    return data;
                });
            }
        }
    }

    [HarmonyPatch(typeof(Game1))]
    [HarmonyPatch("getTimeOfDayString")]
    class GetTimeOfDayPatch
    {
        static bool Prefix()
        {
            return false;
        }

        static void Postfix(int time, ref string __result)
        {
            __result = ModEntry.ConvertTime(time);
        }
    }

    [HarmonyPatch(typeof(StardewValley.Menus.DayTimeMoneyBox))]
    [HarmonyPatch("draw")]
    class DayTimeMoneyBoxPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Algorithm for patching this method:
            // 1 - Find the unique Ldstr instruction with ":"
            // 2 - Once the ":" is found, save the next Stloc_S instruction. It's the instruction to save the variable that we want to modify
            // 3 - Still right after the ":", find the Ldsfld instruction that as "dialogueFont" in operand, we will inject our code right after this instruction

            var instructionsList = instructions.ToList();

            int foundIndex = -1;
            bool foundColon = false;
            bool foundInstruction = false;
            CodeInstruction instructionMemory = null;

            for (var i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];

                if (instruction.opcode == OpCodes.Ldstr && instruction.operand as String == ":")
                {
                    foundColon = true;
                }

                if (foundColon && instruction.opcode == OpCodes.Stloc_S)
                {
                    instructionMemory = instruction;
                    foundColon = false;
                    foundInstruction = true;
                }

                if (foundInstruction && instruction.opcode == OpCodes.Ldsfld && instruction.operand != null && instruction.operand.ToString() == "Microsoft.Xna.Framework.Graphics.SpriteFont dialogueFont")
                {
                    foundIndex = i;
                    foundInstruction = false;
                }
            }

            // Injection details:
            // 1 - Load Game1.timeOfDay on the execution stack
            // 2 - Call our internal method to compute the time
            // 3 - Save the variable with the instruction we have found earlier
            CodeInstruction[] insertions = {
                new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).GetField("timeOfDay")),
                new CodeInstruction(OpCodes.Call, typeof(DayTimeMoneyBoxPatch).GetMethod("ComputeTime")),
                instructionMemory,
            };

            instructionsList.InsertRange(foundIndex + 1, insertions);
            return instructionsList.AsEnumerable();
        }

        public static string ComputeTime(int time)
        {
            return ModEntry.ConvertTime(time);
        }
    }
}