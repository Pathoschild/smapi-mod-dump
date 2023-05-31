/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Archery.Framework.Models.Ammo
{
    public class ArrowTailModel
    {
        public Rectangle Source { get; set; }
        public Vector2 Offset { get; set; }
        public int Amount { get; set; } = 4;
        public int SpawnIntervalInMilliseconds { get; set; } = 50;

        public float? ScaleStep { get; set; }
        public float SpacingStep { get; set; } = 0.25f;
        public float? AlphaStep { get; set; } = 0.1f;
    }
}
