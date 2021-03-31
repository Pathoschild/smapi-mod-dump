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
using ImJustMatt.GarbageDay.API;

namespace ImJustMatt.GarbageDay.Framework.Models
{
    internal class ContentModel : IContent
    {
        public IDictionary<string, IDictionary<string, double>> Loot { get; set; } = new Dictionary<string, IDictionary<string, double>>();
    }
}