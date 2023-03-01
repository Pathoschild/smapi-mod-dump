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
using Microsoft.Xna.Framework;
using System;

namespace FashionSense.Framework.Models.Appearances.Sleeves
{
    public class SleevesModel : AppearanceModel
    {
        public Position BodyPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size SleevesSize { get; set; }
        public bool DrawBeforeShirt { get; set; }
        public bool DrawBeforeBase { get; set; }
        public bool UseShirtColors { get; set; }

        // Old property, has been renamed to DrawBeforeBase
        [Obsolete("Has been renamed to DrawBeforeBase.")]
        public bool DrawBeforeHair { set { DrawBeforeBase = value; } }

        public SkinToneModel ShirtToneMask { get; set; }

        internal bool IsShirtToneMaskColor(Color color)
        {
            if (!HasShirtToneMask() || ShirtToneMask is null)
            {
                return false;
            }

            if (ShirtToneMask.LightTone is not null && color == ShirtToneMask.Lightest)
            {
                return true;
            }
            else if (ShirtToneMask.MediumTone is not null && color == ShirtToneMask.Medium)
            {
                return true;
            }
            else if (ShirtToneMask.DarkTone is not null && color == ShirtToneMask.Darkest)
            {
                return true;
            }

            return false;
        }

        internal bool HasShirtToneMask()
        {
            if (ShirtToneMask is null)
            {
                return false;

            }
            return ShirtToneMask.LightTone is not null || ShirtToneMask.MediumTone is not null || ShirtToneMask.DarkTone is not null;
        }
    }
}
