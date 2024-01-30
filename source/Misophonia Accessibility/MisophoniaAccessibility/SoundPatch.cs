/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility
{
    /// <summary>
    /// Harmony patch for the sound
    /// </summary>
    internal class SoundPatch
    {
        /// <summary>
        /// Modifies the Game1.playSound method to not play the eating sound
        /// </summary>
        /// <param name="cueName">Sound that will be played from Stardew Valley</param>
        internal static bool PatchSound(string cueName)
        {
            return MisophoniaAccessibilityMod.DisabledCodeSounds.TryGetValue(key: cueName, value: out _);
        }
    }
}
