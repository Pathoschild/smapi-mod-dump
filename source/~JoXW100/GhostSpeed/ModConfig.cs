/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace GhostSpeed
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public float SpeedMultiplier { get; set; } = 2.0f;
        public int TilesKnockedBack { get; set; } = 6;
    }
}
