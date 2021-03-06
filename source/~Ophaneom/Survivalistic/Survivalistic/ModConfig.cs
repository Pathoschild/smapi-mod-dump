/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Survivalistic
{
    class ModConfig
    {
        public string Language { get; set; }
        public string Position { get; set; }
        public int CustomPositionX { get; set; }
        public int CustomPositionY { get; set; }
        public Color HungerBarColor { get; set; }
        public Color HungerSaturationColor { get; set; }
        public Color ThirstBarColor { get; set; }
        public Color ThirstSaturationColor { get; set; }
    }
}
