/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.StardewWidgets.Framework.Enums;

using NetEscapades.EnumGenerators;

/// <summary>Represents the type widget.</summary>
[EnumExtensions]
internal enum WidgetType
{
    /// <summary>Represents a bar widget.</summary>
    Bar,

    /// <summary>Represents a button widget.</summary>
    Button,

    /// <summary>Represents a grid widget.</summary>
    Grid,

    /// <summary>Represents a highlight widget.</summary>
    Highlight,

    /// <summary>Represents a icon widget.</summary>
    Icon,

    /// <summary>Represents a stack widget.</summary>
    Stack,

    /// <summary>Represents a tooltip widget.</summary>
    Tooltip,
}