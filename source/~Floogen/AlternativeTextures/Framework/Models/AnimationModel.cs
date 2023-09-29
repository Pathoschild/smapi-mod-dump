/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Enums;
using System;

namespace AlternativeTextures.Framework.Models
{
    public class AnimationModel
    {
        public int Frame { get; set; }
        public int Duration { get; set; } = 1000;
        [Obsolete("Unused. Will need to update AlternativeTextureModel.GetAnimationDataAtIndex to accept Object.minutesUntilReady.", true)]
        public FrameType Type { get; set; } = FrameType.Default;
    }
}
