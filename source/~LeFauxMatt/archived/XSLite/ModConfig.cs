/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#nullable disable

namespace XSLite;

using System.Collections.Generic;

internal class ModConfig
{
    public int Capacity { get; set; }

    public HashSet<string> EnabledFeatures { get; set; } = new();
}