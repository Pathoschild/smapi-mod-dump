/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Models.Appearances.Generic;
using Microsoft.Xna.Framework;

namespace FashionSense.Framework.Models.Appearances.Body
{
    public class BodyModel : AppearanceModel
    {
        public int EyeBackgroundPosition { get; set; }
        public int EyePosition { get; set; }
        public SkinToneModel SkinTone { get; set; }
        public Rectangle? EyeBaseSourceRectangle { get; set; }
        public PortraitModel Portrait { get; set; }

        public int HeightOffset { get; set; }
        public int? AccessoryOffset { get; set; }
        public int? HeadOffset { get; set; }
        public int? LegOffset { get; set; }
        public int? ShoeOffset { get; set; }
        public int? BodyOffset { get; set; }
        public int? ArmsOffset { get; set; }
        public Size BodySize { get; set; }

        internal int GetFeatureOffset(IApi.Type type, int defaultValue = 0)
        {
            switch (type)
            {
                case IApi.Type.Accessory:
                    return AccessoryOffset is not null ? AccessoryOffset.Value : defaultValue;
                case IApi.Type.Hat:
                case IApi.Type.Hair:
                    return HeadOffset is not null ? HeadOffset.Value : defaultValue;
                case IApi.Type.Pants:
                    return LegOffset is not null ? LegOffset.Value : defaultValue;
                case IApi.Type.Shoes:
                    return ShoeOffset is not null ? ShoeOffset.Value : GetFeatureOffset(IApi.Type.Pants, defaultValue);
                case IApi.Type.Shirt:
                    return BodyOffset is not null ? BodyOffset.Value : defaultValue;
                case IApi.Type.Sleeves:
                    return ArmsOffset is not null ? ArmsOffset.Value : defaultValue;
                default:
                    return defaultValue;
            }
        }
    }
}
