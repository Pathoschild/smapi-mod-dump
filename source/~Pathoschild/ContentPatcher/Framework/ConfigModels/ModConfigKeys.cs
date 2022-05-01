/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ContentPatcher.Framework.ConfigModels
{
    /// <summary>A set of parsed key bindings.</summary>
    internal class ModConfigKeys
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keys which toggle the display of debug information.</summary>
        public KeybindList ToggleDebug { get; set; } = new(SButton.F3);

        /// <summary>The keys which switch to the previous texture.</summary>
        public KeybindList DebugPrevTexture { get; set; } = new(SButton.LeftControl);

        /// <summary>The keys which switch to the next texture.</summary>
        public KeybindList DebugNextTexture { get; set; } = new(SButton.RightControl);
    }
}
