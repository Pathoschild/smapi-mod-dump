/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prince-Leto/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace MultiplayerShaftFix
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
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
    }

    [HarmonyPatch(typeof(MineShaft))]
    [HarmonyPatch("enterMineShaft")]
    class PatchEnterMineShaft
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Aglorithm for patching this method:
            // 1 - Find the latest Stloc_0 (i.e. level variable that have been computed)
            // 2 - Compute another value based on the current day and the current mine level
            // 3 - Replace the previous value

            var instructionsList = instructions.ToList();

            int lastIndex = -1;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Stloc_0)
                {
                    lastIndex = i;
                }
            }

            // Injection details:
            // 1 - Load `this` onto the stack
            // 2 - Call our internal method
            // 3 - Save the variable
            CodeInstruction[] insertions = {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, typeof(PatchEnterMineShaft).GetMethod("ComputeLevels")),
                new CodeInstruction(OpCodes.Stloc_0)
            };

            // Inject our code after the next instruction of our variable initialization
            instructionsList.InsertRange(lastIndex + 2, insertions);

            return instructionsList.AsEnumerable();
        }

        static int ComputeInitializer(MineShaft instance)
        {
            // Use `* 100` on DaysSinceStart because I don't want to add both value directly
            // If I did, you will be able to predict the values of the shaft, because their value will change from 1 floor everyday
            // Example: If on a day you find a shaft on the 8th floor, which makes you fall 5 levels,
            // the following day if by luck you find a shaft on the 7th floor, you'll know that you will fall 5 levels again.
            // That would be less funny

            // The `* 1000` is because... why not? ¯\_(ツ)_/¯
            return (SDate.Now().DaysSinceStart * 100 + instance.mineLevel) * 1000;
        }

        public static int ComputeLevels(MineShaft instance)
        {
            int initializer = PatchEnterMineShaft.ComputeInitializer(instance);

            // Initialize the randomizer with today's date and the current mine level
            // It can be improved, but at least it works quite well like that
            Random mineRandom = new Random(initializer);

            // Then, the two next call on random will always be the same for the same day and the same mine level
            int level = mineRandom.Next(3, 9);
            if (mineRandom.NextDouble() < 0.1)
            {
                level = level * 2 - 1;
            }

            return level;
        }
    }
}