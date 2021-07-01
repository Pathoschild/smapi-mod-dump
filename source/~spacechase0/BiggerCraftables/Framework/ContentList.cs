/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace BiggerCraftables.Framework
{
    internal class ContentList
    {
        public class Entry
        {
            [JsonIgnore]
            public Texture2D Texture { get; set; }

            public string Name { get; set; }
            public string Image { get; set; }
            public int Width { get; set; }
            public int Length { get; set; }
        }

        public List<Entry> BiggerCraftables { get; set; } = new();
    }
}
