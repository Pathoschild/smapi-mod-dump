/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace AeroCore.Patches
{
    [HarmonyPatch]
    internal class LightingQuality
    {
        [HarmonyPatch(typeof(OptionsPlusMinus), MethodType.Constructor, new Type[] {
        typeof(string), typeof(int), typeof(List<string>), typeof(List<string>), typeof(int), typeof(int)})]
        [HarmonyPrefix]
        internal static void alterLightingOptions(int whichOption, ref List<string> options, ref List<string> displayOptions)
        {
            if (whichOption == 25)
            {
                options.Add("Ultra");
                displayOptions.Add(ModEntry.i18n.Get("misc.lightQuality.ultra"));
                options.Add("Extreme");
                displayOptions.Add(ModEntry.i18n.Get("misc.lightQuality.extreme"));
            }
        }

        [HarmonyPatch(typeof(Options),nameof(Options.changeDropDownOption))]
        [HarmonyPrefix]
        internal static bool setExtremeLighting(Options __instance, int which, string value)
        {
            if (which == 25 && value == "Extreme")
            {
                __instance.lightingQuality = 4;
                Game1.overrideGameMenuReset = true;
                Program.gamePtr.refreshWindowSettings();
                Game1.overrideGameMenuReset = false;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Options),nameof(Options.setPlusMinusToProperValue))]
        [HarmonyPrefix]
        internal static bool loadExtremeLighting(Options __instance, OptionsPlusMinus plusMinus)
        {
            if(plusMinus.whichOption == 25 && __instance.lightingQuality == 4)
            {
                for (int j = 0; j < plusMinus.options.Count; j++)
                {
                    if (plusMinus.options[j].Equals("Extreme"))
                    {
                        plusMinus.selected = j;
                        break;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
