/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances.Generic;

namespace FashionSense.Framework.Models.Appearances.Hair
{
    public class HairModel : AppearanceModel
    {
        public Position HeadPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size HairSize { get; set; }

        public override bool HideWhileSwimming { get; set; } = false;
        public override bool HideWhileWearingBathingSuit { get; set; } = false;
    }
}
