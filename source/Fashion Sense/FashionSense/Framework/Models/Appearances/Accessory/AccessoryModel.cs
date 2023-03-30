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
using System;

namespace FashionSense.Framework.Models.Appearances.Accessory
{
    public class AccessoryModel : AppearanceModel
    {
        internal enum Type
        {
            Unknown,
            Primary,
            Secondary,
            Tertiary
        }

        public Position HeadPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size AccessorySize { get; set; }
        public bool DrawBehindHead { get; set; }
        public bool DrawAfterPlayer { get; set; }
        public bool DrawAfterSleeves { get; set; }

        public override bool HideWhileSwimming { get; set; } = false;
        public override bool HideWhileWearingBathingSuit { get; set; } = false;

        // Old property, has been renamed to DrawBehindHead
        [Obsolete("Has been renamed to DrawBehindHead.")]
        public bool DrawBeforeHair { set { DrawBehindHead = value; } }

        // Old property, has been renamed to DrawAfterPlayer
        [Obsolete("Has been renamed to DrawAfterPlayer.")]
        public bool DrawBeforePlayer { set { DrawAfterPlayer = value; } }

        internal Type Priority { get; set; }
    }
}
