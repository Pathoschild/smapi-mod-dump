/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/BiggerCraftables
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiggerCraftables
{
    public class ContentList
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

        public List<Entry> BiggerCraftables { get; set; } = new List<Entry>();
    }
}
