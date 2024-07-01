/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
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
        public static readonly Color BILLBOARD_COLOR = new Color(251/255f, 235/255f, 194/255f); // The color of the paper on the calendar


        public static IEnumerable<CodeInstruction> Billboard_Constructor_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Billboard.ctor");

            bool first = false;

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count - 2; i++)
            {
                if (!first && codes[i].opcode == OpCodes.Ldloc_3 && codes[i+1].opcode == OpCodes.Ldloc_2 && codes[i+2].opcode == OpCodes.Callvirt && codes[i+2].operand is MethodInfo info && info == AccessTools.Method(typeof(Billboard), nameof(Billboard.GetEventsForDay)))
                {
                    // There is a loop that goes i from 1-28 that generates the events for each day. This maps that i to the correct day if this isn't the first page of the calendar so we get the events for the right day (i.e. maps 1 to 29 on the second page).
                    SMonitor.Log("Mapping the day for events");
                    codes.Insert(i + 1, CodeInstruction.Call(typeof(ModEntry), nameof(GetDay)));

                    //first = true;
                    break;
                }

                // This is unnecessary because it just changes the IClickableComponent names to the correct date, which doesn't really matter so I'm not doing it to remove points of failure
                /*
                if(first && codes[i].opcode == OpCodes.Ldloca_S && codes[i].operand is LocalBuilder builder && builder.LocalIndex == 3 && codes[i+1].opcode == OpCodes.Call && codes[i+1].operand is MethodInfo info1 && info1 == AccessTools.Method(typeof(int), nameof(int.ToString)))
                {
                    SMonitor.Log("Updating the display date");

                    codes[i].opcode = OpCodes.Ldloc_3;
                    codes[i].operand = null;

                    codes[i + 1].operand = AccessTools.Method(typeof(ModEntry), nameof(GetDisplayDay));
                    break;
                }*/
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Gets the day number for the dayOnThisPage'th square of the calendar (identical on the first page, dayOnThisPage + 28 on the second,...)
        /// </summary>
        private static int GetDay(int dayOnThisPage)
        {
            return ((Game1.dayOfMonth-1) / 28) * 28 + dayOnThisPage;
        }

        /// <summary>
        /// Gets the day to display on the dayOnThisPage'th slot of the calendar.
        /// </summary>
        private static string GetDisplayDay(int dayOnThisPage)
        {
            int day = GetDay(dayOnThisPage);
            return day <= Config.DaysPerMonth ? day.ToString() : "";
        }

        public static IEnumerable<CodeInstruction> Billboard_draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Billboard.draw");

            bool first = true;

            // There is a loop that goes i from 1-28 that if the day (i) is less than the current day, grays it out, and if it is equal, draws a symbol on it. This maps that i to the correct day if it isn't the first page of the calendar.

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && (FieldInfo)codes[i].operand is FieldInfo info && info == AccessTools.Field(typeof(Game1), nameof(Game1.dayOfMonth)) && codes[i + 1].opcode == OpCodes.Ldloc_2 && codes[i + 2].opcode == OpCodes.Ldc_I4_1 && codes[i + 3].opcode == OpCodes.Add && (codes[i + 4].opcode == OpCodes.Ble_S || codes[i+4].opcode == OpCodes.Bne_Un_S))
                {
                    SMonitor.Log("Removing greyed out date covering " + (first ? "greater" : "equal"));
                    codes.Insert(i+2, CodeInstruction.Call(typeof(ModEntry), nameof(GetDay)));

                    if(first)
                    {
                        first = false;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}