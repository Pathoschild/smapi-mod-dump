/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/tophatsquid/sdv-configurable-junimo-kart
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StardewValley;
using StardewValley.Locations;

namespace ConfigurableJunimoKart
{
    public class ModConfig
    {
        public bool infinite_jumps { get; set; } = false;
        public bool infinite_lives { get; set; } = false;
        public float speed_multiplier { get; set; } = 1f;
        public float gravity { get; set; } = 300f;
        public float jump_strength { get; set; } = 150f;

    }
}
