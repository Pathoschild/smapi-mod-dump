/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace RemovableHorseHats
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/

        /// <summary>The keys that allow you to remove the hat from your horse when they're held down.</summary>
        public string KeysToRemoveHat { get; set; } = $"{SButton.LeftShift} {SButton.RightShift}";
    }
}
