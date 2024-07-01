/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowBuff
{
    public record LightSourceData
    {
        public int TextureId { get; set; }

        public int Radius { get; set; }

        public Color Color { get; set; }

        public int Duration { get; set; }

        public bool PrismaticColor { get; set; }

        public string Source { get; set; }
    }
}
