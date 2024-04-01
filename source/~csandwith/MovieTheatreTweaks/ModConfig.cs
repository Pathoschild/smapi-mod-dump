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
using StardewModdingAPI;
using System.Collections.Generic;

namespace MovieTheatreTweaks
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool RemoveCraneMan { get; set; } = true;
        public int CloseTime { get; set; } = 2100;
        public int OpenTime { get; set; } = 900;
    }
}
