/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace AFKTimePause
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool FreezeGame { get; set; } = true;
        public int SecondsTilAFK { get; set; } = 60;
        public bool ShowAFKText { get; set; } = false;
        public string AFKText { get; set; } = "AFK";
    }
}
