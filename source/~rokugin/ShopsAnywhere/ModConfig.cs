/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ShopsAnywhere {
    internal class ModConfig {

        public ModConfigKeys Controls { get; set; } = new();
        public int AdvShopButtonOffsetX { get; set; } = 0;
        public int AdvShopButtonOffsetY { get; set; } = 0;

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context) {
            Controls ??= new ModConfigKeys();
        }

    }
}
