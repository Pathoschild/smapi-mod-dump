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
