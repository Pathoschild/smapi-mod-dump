/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewValley;

#endregion using directives

internal class HostState
{
    internal HashSet<long> PoachersInAmbush { get; } = new();
    internal HashSet<long> AggressivePipers { get; } = new();
    internal Dictionary<long, Farmer> FakeFarmers { get; } = new();
}