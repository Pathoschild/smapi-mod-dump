/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Enums;

using NetEscapades.EnumGenerators;
using StardewValley.Menus;

/// <summary>
///     <see cref="ClickableTextureComponent" /> that are added to the <see cref="ItemGrabMenu" />.
/// </summary>
[EnumExtensions]
public enum ComponentType
{
    /// <summary>A custom component.</summary>
    Custom,

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
}