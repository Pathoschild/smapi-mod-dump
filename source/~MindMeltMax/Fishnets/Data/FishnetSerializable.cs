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

namespace Fishnets.Data
{
    [Obsolete("Only for backwards compatibility, will be removed in a future version")]
    public class FishNetSerializable
    {
        public long Owner { get; set; } = 0L;

        public string Bait { get; set; } = "";

        public int BaitQuality { get; set; } = 0;

        public string ObjectName { get; set; } = "";

        public string ObjectId { get; set; } = "";

        public int ObjectStack { get; set; } = -1;

        public int ObjectQuality { get; set; } = 0;

        public bool IsJAObject { get; set; } = false;

        public bool IsDGAObject { get; set; } = false;

        public Vector2 Tile { get; set; }

        public FishNetSerializable() { }
    }
}
