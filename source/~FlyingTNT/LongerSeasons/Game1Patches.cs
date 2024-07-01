/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Common.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LongerSeasons
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        /// <summary>
        /// Replaces the part of Game1._newDayAfterFade that changes the season if the day is over 28
        /// </summary>
        public static IEnumerable<CodeInstruction> Game1__newDayAfterFade_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Game1._newDayAfterFade");

            bool preventSeasonChange = true;
            bool updateSpecialOrderBoard = true;

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (preventSeasonChange && codes[i].opcode == OpCodes.Ldsfld && (FieldInfo)codes[i].operand == typeof(Game1).GetField(nameof(Game1.dayOfMonth), BindingFlags.Public | BindingFlags.Static) && codes[i + 1].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i + 1].operand == 28)
                {
                    // Replaces the number 28 with a call to GetDaysPerMonth()
                    SMonitor.Log($"Changing days per month");
                    codes[i + 1].opcode = OpCodes.Call;
                    codes[i + 1].operand = AccessTools.Method(typeof(Utilities), nameof(Utilities.GetDaysPerMonth));


                    if (codes[i+5].opcode == OpCodes.Ldsfld && codes[i+5].operand is FieldInfo info && info == AccessTools.Field(typeof(Game1), nameof(Game1.season)) &&
                        codes[i+9].opcode == OpCodes.Br_S && codes[i+9].operand is Label breakToLabel)
                    {
                        // Adds a brfalse that will skip the section of code that increments the current season. The brfalse is only hit if there are multiple months per season and the current season is not in its last month.
                        SMonitor.Log("Changing conditions for changing the season");
                        codes.Insert(i + 5, new CodeInstruction(OpCodes.Brfalse, breakToLabel));
                        codes.Insert(i + 5, new CodeInstruction(CodeInstruction.Call(typeof(ModEntry), nameof(IncrementSeasonMonth))));
                    }

                    preventSeasonChange = false;
                }

                // The special order board is hardcoded to update on the 1st, 8th, 15th, and 22nd. We find those checks and replace them with a call to ShouldUpdateQuestBoard
                if (updateSpecialOrderBoard && i>9 && codes[i-9].opcode == OpCodes.Ldc_I4_1 && codes[i-6].opcode == OpCodes.Ldc_I4_8 && codes[i-3].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i-3].operand == 15 && codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 22 && codes[i+1].opcode == OpCodes.Bne_Un_S && codes[i+1].operand is Label)
                {
                    SMonitor.Log("Making the quest board update after the 22nd");

                    for(int j = -10; j < -1; j++)
                    {
                        codes[i + j].opcode = OpCodes.Nop;
                        codes[i + j].operand = null;
                    }

                    codes[i] = CodeInstruction.Call(typeof(ModEntry), nameof(ShouldUpdateQuestBoard));
                    codes[i + 1].opcode = OpCodes.Brfalse;

                    updateSpecialOrderBoard = false;
                }
            }

            // For some reason, ILSpy really didn't want to show me the bytecodes for this method so I had to print them out.
            /*foreach(var code in codes)
            {
                SMonitor.Log($"{((code.labels?.Count ?? 0) > 0 ? code.labels[0] : null)}|{code.opcode}|{code.operand}");
            }*/

            return codes.AsEnumerable();
        }

        private static void Game1__newDayAfterFade_Prefix()
        {
            SMonitor.Log($"dom {Game1.dayOfMonth}, year {Game1.year}, season {Game1.currentSeason}");
        }

        /// <summary>
        /// This is added in the Game1._newDayAfterFade transpiler. If it returns false, the code that increments the season will be skipped.
        /// </summary>
        /// <returns>True if the current season should be incremented; i.e. the current month is the last in the season.</returns>
        protected static bool IncrementSeasonMonth()
        {
            bool shouldIncrementSeason = CurrentSeasonMonth >= Config.MonthsPerSeason;

            CurrentSeasonMonth = shouldIncrementSeason ? 1 : CurrentSeasonMonth + 1;

            return shouldIncrementSeason;
        }

        private static bool ShouldUpdateQuestBoard(int day)
        {
            return (day - 1) % 7 == 0;
        }
    }
}