/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerMod
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool RightSide { get; set; } = true;
        public bool Bottom { get; set; } = true;
        public int XOffset { get; set; } = 64;
        public int YOffset { get; set; } = 64;
        public int Width { get; set; } = 720;
        public int Height { get; set; } = 480;
        public bool PhoneApp { get; set; } = true;
    }
}
