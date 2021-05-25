/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

namespace CustomCompanions.Framework.Models.Companion
{
    public class ShadowModel
    {
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float Scale { get; set; }
        public int Alpha { get; set; } = 255;

        public override string ToString()
        {
            return $"[Offset: {OffsetX}, {OffsetY} | Scale: {Scale} | Alpha: {Alpha}]";
        }
    }
}
