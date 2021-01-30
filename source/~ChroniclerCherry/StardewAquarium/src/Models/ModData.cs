/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace StardewAquarium.Models
{
    public class ModData
    {
        public int LastDonatedFishCoordinateX { get; set; }
        public int LastDonatedFishCoordinateY { get; set; }
        public string ExteriorMapName { get; set; }

        public string[] ConversationTopicsOnDonate { get; set; }

        public float DolphinChance { get; set; }

        public Rectangle DolphinRange { get; set; }

        public int DolphinAnimationFrames { get; set; }

    }
}
