/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace CommonHarmony.Enums
{
    using StardewValley.Menus;

    /// <summary>
    ///     Default side buttons alongside the <see cref="ItemGrabMenu" />
    /// </summary>
    internal enum ButtonType
    {
        /// <summary>The Organize Button.</summary>
        OrganizeButton,

        /// <summary>The Fill Stacks Button.</summary>
        FillStacksButton,

        /// <summary>The Color Picker Toggle Button.</summary>
        ColorPickerToggleButton,

        /// <summary>The Special Button.</summary>
        SpecialButton,

        /// <summary>The Junimo Note Icon.</summary>
        JunimoNoteIcon,

        /// <summary>Custom non-vanilla button.</summary>
        Custom,
    }
}