/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The runtime state for Tool variables.</summary>
internal sealed class State
{
    internal List<Shockwave> Shockwaves { get; } = new();

    internal Dictionary<Type, SelectableTool?> SelectableToolByType { get; } = new();
}
