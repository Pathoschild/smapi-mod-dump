/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Enums;

using NetEscapades.EnumGenerators;
using StardewValley.Menus;

/// <summary>The type of mod integration.</summary>
[EnumExtensions]
internal enum IntegrationType
{
    /// <summary>Opens an <see cref="IClickableMenu" /> from the mod.</summary>
    Menu,

    /// <summary>Invokes a method from the mod.</summary>
    Method,

    /// <summary>Issue a keybind.</summary>
    Keybind,
}