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
    public class LightModel
    {
        public int[] Color { get; set; }
        public float Radius { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public int PulseSpeed { get; set; }
        public float PulseMinRadius { get; set; }

        public override string ToString()
        {
            return $"[Color: {string.Join(",", Color)} | Radius: {Radius} | Offset: {OffsetX}, {OffsetY} | PulseSpeed: {PulseSpeed} | PulseMinRadius: {PulseMinRadius}]";
        }
    }
}
