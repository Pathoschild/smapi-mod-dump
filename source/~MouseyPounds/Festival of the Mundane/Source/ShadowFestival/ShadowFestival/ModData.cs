/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MouseyPounds/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ShadowFestival
{
    class ModData
    {
        public List<string> CalmingHats { get; set; } = new List<string> {
            "Imposing Mask", "Shamanic Mask", "Shady Mask", "Shady Bowed Mask"
        };
        public List<string> OtherHats { get; set; } = new List<string> {
            "Strange Bun Hat"
        };
    }
}
