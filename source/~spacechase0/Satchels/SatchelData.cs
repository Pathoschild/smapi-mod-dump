/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satchels
{
    public class SatchelData
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Texture { get; set; } = "spacechase0.Satchels/satchels.png";
        public int TextureIndex { get; set; } = 0;

        public int Capacity { get; set; }
        public int MaxUpgrades { get; set; }
    }
}
