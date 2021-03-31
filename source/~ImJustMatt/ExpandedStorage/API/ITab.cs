/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace ImJustMatt.ExpandedStorage.API
{
    public interface ITab
    {
        /// <summary>Image to display for tab, will search asset folder first and default next.</summary>
        string TabImage { get; set; }

        /// <summary>When specified, tab will only show the listed item/category IDs.</summary>
        HashSet<string> AllowList { get; set; }

        /// <summary>When specified, tab will show all/allowed items except for listed item/category IDs.</summary>
        HashSet<string> BlockList { get; set; }
    }
}