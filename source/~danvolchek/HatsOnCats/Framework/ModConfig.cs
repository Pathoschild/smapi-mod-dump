/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.Collections.Generic;
using HatsOnCats.Framework.Configuration;
using Microsoft.Xna.Framework;

namespace HatsOnCats.Framework
{
    internal class ModConfig
    {
        public IDictionary<string, IDictionary<Frame, Offset>> Offsets { get; set; } = new Dictionary<string, IDictionary<Frame, Offset>>();
    }
}
