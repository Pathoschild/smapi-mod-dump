/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/

using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility
{
    [HarmonyPatch(typeof(Game1))]
    [HarmonyPatch("playSound")]
    public static class EatSoundPatch
    {
        /// <summary>
        /// Modifies the Game1.playSound method to not play the eating sound
        /// </summary>
        /// <param name="cueName">Sound that will be played from Stardew</param>
        [HarmonyPrefix]
        public static bool Prefix(string cueName)
        {
           if (MisophoniaAccessibilityMod.DisabledSounds.ContainsKey(cueName) &&
                    MisophoniaAccessibilityMod.DisabledSounds[cueName] == true)
                return false;
            else
                return true;
        }
    }
}
