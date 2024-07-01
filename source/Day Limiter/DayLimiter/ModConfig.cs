/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tadfoster/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace DayLimiter
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = false;
        public bool ExitToTitle { get; set; } = false;
        public int DayLimitCount { get; set; } = 3;
    }
}