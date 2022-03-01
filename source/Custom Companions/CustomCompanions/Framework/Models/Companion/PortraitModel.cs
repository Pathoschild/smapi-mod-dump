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
    public class PortraitModel
    {
        public int FrameSizeWidth { get; set; } = 64;
        public int FrameSizeHeight { get; set; } = 64;
        public int FrameIndex { get; set; }
        public string PortraitDisplayName { get; set; }

        public override string ToString()
        {
            return $"[FrameIndex: {FrameIndex} | FrameSize: {FrameSizeWidth}x{FrameSizeHeight} | PortraitDisplayName: {PortraitDisplayName}]";
        }
    }
}
