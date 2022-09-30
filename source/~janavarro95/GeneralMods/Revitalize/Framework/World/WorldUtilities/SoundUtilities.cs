/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using static Omegasis.Revitalize.Framework.Constants.Enums;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{

    /// <summary>
    /// Utilities for playing various sounds for the game.
    /// </summary>
    public static class SoundUtilities
    {

        /// <summary>
        /// Gets the name of the StardewValley Sound.
        /// </summary>
        /// <param name="stardewSound"></param>
        /// <returns></returns>
        public static string GetSoundNameFromStardewSound(StardewSound stardewSound)
        {
            return Enum.GetName<StardewSound>(stardewSound);
        }

        public static void PlaySound(StardewSound stardewSound)
        {
            string soundName = GetSoundNameFromStardewSound(stardewSound);
            Game1.playSound(soundName);
        }

        public static void PlaySound(this GameLocation GameLocation, StardewSound stardewSound)
        {
            string soundName = GetSoundNameFromStardewSound(stardewSound);
            GameLocation.playSound(GetSoundNameFromStardewSound(stardewSound));
        }

    }
}
