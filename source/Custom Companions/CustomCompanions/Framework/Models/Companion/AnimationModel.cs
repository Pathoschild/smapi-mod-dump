/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using System.Collections.Generic;

namespace CustomCompanions.Framework.Models.Companion
{
    public class AnimationModel
    {
        public int StartingFrame { get; set; }
        public int NumberOfFrames { get; set; }
        public int Duration { get; set; }
        public List<ManualFrameModel> ManualFrames { get; set; }
        public IdleAnimationModel IdleAnimation { get; set; } = new IdleAnimationModel();

        public override string ToString()
        {
            return $"[StartingFrame: {StartingFrame} | NumberOfFrames: {NumberOfFrames} | Duration: {Duration} | ManualFrames: {(ManualFrames is null ? null : string.Join(",", ManualFrames))} | IdleAnimation: {IdleAnimation}]";
        }
    }
}
