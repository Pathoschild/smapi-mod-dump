/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DialogueNewLine
{
    public partial class ModEntry
    {
        private static char specialCharacter = '¶';
        private static bool newLine;

        [HarmonyPatch(typeof(SpriteText), nameof(SpriteText.positionOfNextSpace))]
        public class SpriteText_positionOfNextSpace_Patch
        {
            private static bool Prefix(string s, int index, int currentXPosition, int accumulatedHorizontalSpaceBetweenCharacters, ref int __result)
            {
                if (!Config.EnableMod || index == 0 || s[index - 1] != specialCharacter)
                    return true;
                __result = int.MaxValue;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(SpriteText), "IsSpecialCharacter")]
        public class SpriteText_IsSpecialCharacter_Patch
        {
            public static bool Prefix(char c, ref bool __result)
            {
                if(Config.EnableMod && c == specialCharacter)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}