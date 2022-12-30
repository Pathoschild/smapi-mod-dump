/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class State
{
    internal List<Shockwave> Shockwaves { get; } = new();
}
