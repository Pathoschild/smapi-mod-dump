/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

namespace FashionSense.Framework.Models.Appearances.Hair
{
    public class HairContentPack : AppearanceContentPack
    {
        public HairModel BackHair { get; set; }
        public HairModel RightHair { get; set; }
        public HairModel FrontHair { get; set; }
        public HairModel LeftHair { get; set; }

        internal HairModel GetHairFromFacingDirection(int facingDirection)
        {
            HairModel hairModel = null;
            switch (facingDirection)
            {
                case 0:
                    hairModel = BackHair;
                    break;
                case 1:
                    hairModel = RightHair;
                    break;
                case 2:
                    hairModel = FrontHair;
                    break;
                case 3:
                    hairModel = LeftHair;
                    break;
            }

            return hairModel;
        }

        internal override void LinkId()
        {
            if (BackHair is AppearanceModel backModel && backModel is not null)
            {
                backModel.Pack = this;
            }
            if (RightHair is AppearanceModel rightModel && rightModel is not null)
            {
                rightModel.Pack = this;
            }
            if (FrontHair is AppearanceModel frontModel && frontModel is not null)
            {
                frontModel.Pack = this;
            }
            if (LeftHair is AppearanceModel leftModel && leftModel is not null)
            {
                leftModel.Pack = this;
            }
        }
    }
}
