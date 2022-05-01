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

namespace Pathoschild.Stardew.RotateToolbar.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to deselect the current slot after rotating the toolbar.</summary>
        public bool DeselectItemOnRotate { get; set; } = false;

        /// <summary>The key bindings.</summary>
        public ModConfigKeys Controls { get; set; } = new();
    }
}
